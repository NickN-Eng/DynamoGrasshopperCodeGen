using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;


namespace DGCodeGen.Engine
{
    public static class SyntaxHelper
    {
        public static T QuickParse<T>(string code) where T : SyntaxNode
        {
            var tree = CSharpSyntaxTree.ParseText(code, new CSharpParseOptions(kind: SourceCodeKind.Script));
            var debugNodeList = tree.GetRoot().DescendantNodes().ToList();
            var expressionNode = tree.GetRoot().DescendantNodes().FirstOrDefault(node => node is T);
            if (expressionNode == null)
                throw new Exception($"Code string does not contain {typeof(T).Name} node. Syntax contained is "
                    + string.Join(",", tree.GetRoot().ChildNodes().Select(node => node.Kind().ToString()).ToArray()));

            return (T)expressionNode;
        }

        public static ExpressionSyntax GetExpression(string expression)
        {
            var tree = CSharpSyntaxTree.ParseText(expression, new CSharpParseOptions(kind: SourceCodeKind.Script));
            var debugNodeList = tree.GetRoot().DescendantNodes().ToList();
            var expressionNode = tree.GetRoot().DescendantNodes().FirstOrDefault(node => node is ExpressionSyntax);
            if (expressionNode == null)
                throw new Exception("Expression string does not contain an expression syntax node. Sytax contained is "
                    + string.Join(",", tree.GetRoot().ChildNodes().Select(node => node.Kind().ToString()).ToArray()));

            return (ExpressionSyntax)expressionNode;
        }

        public static void TestDelegatesWithReturnStatements()
        {
            var func = new Func<int>(() => { if (1 > 1) { return 1; } else { return 2; } });
        }



        public static NameSyntax AutoChainedName(params string[] parts)
        {
            if (parts.Length == 1) return IdentifierName(parts[0]);

            var qualName = QualifiedName(IdentifierName(parts[0]), IdentifierName(parts[1]));
            for (int i = 2; i < parts.Length; i++)
            {
                qualName = QualifiedName(qualName, IdentifierName(parts[i]));
            }
            return qualName;
        }

        public static SyntaxTriviaList CommentTriv(string comment) => TriviaList(Comment(comment));
        public static SyntaxTriviaList NewlineAndCommentTriv(string comment)
        {
            return TriviaList( Comment(comment));
        }

        public static PredefinedTypeSyntax PredefType(SyntaxKind syntaxKind) =>     PredefinedType(Token(syntaxKind));
        public static TypeSyntax ListType(string genericArg) =>                     GenericName(Identifier("List")).WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList(NamedType(genericArg))));
        public static TypeSyntax ListType(TypeSyntax genericType) =>                GenericName(Identifier("List")).WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList(genericType)));
        public static TypeSyntax NamedType(string typeName) =>                      IdentifierName(typeName);
        public static ArgumentSyntax StringLitArg(string str) =>                    Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(str)));
        public static ArgumentSyntax NumberArg(int value) =>                        Argument(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(value)));
        public static ArgumentSyntax NumberArg(double value) =>                     Argument(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(value)));
        public static ArgumentSyntax NamedArg(string variableName) =>               Argument(IdentifierName(variableName));
        public static ArgumentSyntax NamedArgWithRef(string variableName) =>        Argument(IdentifierName(variableName)).WithRefKindKeyword(Token(SyntaxKind.RefKeyword));
        public static AttributeArgumentSyntax AttrStringLitArg(string str) =>       AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(str)));
        public static AttributeArgumentSyntax AttrNumberArg(int value) =>           AttributeArgument(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(value)));
        public static AttributeArgumentSyntax AttrNumberArg(double value) =>        AttributeArgument(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(value)));
        public static AttributeArgumentSyntax AttrNamedArg(string variableName) =>  AttributeArgument(IdentifierName(variableName));


        public static ArgumentListSyntax ArgList(params ArgumentSyntax[] args) => ArgumentList(SeparatedList(args));
        public static AttributeArgumentListSyntax AttrArgList(params AttributeArgumentSyntax[] args) => AttributeArgumentList(SeparatedList(args));

        /// <summary>
        /// Create a parameter list for a method call: 
        /// (name1, name2 etc...). Leaving empty creates emtpy list ()
        /// </summary>
        public static ArgumentListSyntax ArgList(params string[] argNames)
        {
            return ArgumentList(SeparatedList(argNames.Select(an => NamedArg(an)).ToArray()));
        }

        /// <summary>
        /// Create a parameter list for a method declaration: 
        /// (type1 name1, type2 name2, etc...). 
        /// Leaving empty creates emtpy list ()
        /// </summary>
        public static ParameterListSyntax ParamList(params (string name, TypeSyntax typeSyn)[] parameterPairs)
        {
            var parameters = parameterPairs.Select(p => Parameter(Identifier(p.name)).WithType(p.typeSyn));
            return ParameterList(SeparatedList(parameters));
        }

        /// <summary> Create an attribute list </summary>
        public static SyntaxList<AttributeListSyntax> AttrList(params AttributeListSyntax[] attributes) => List(attributes);

        /// <summary> Create an attribute list </summary>
        public static SyntaxList<SyntaxNode> NodeList(params SyntaxNode[] nodes) => List(nodes);

        /// <summary>
        /// List<listType> variableName = new List<listType>();
        /// </summary>
        public static StatementSyntax NewListVariableDeclaration(TypeSyntax listType, string variableName)
        {
            var genericList = ListType(listType);
            //var listTypeArg = TypeArgumentList(SingletonSeparatedList(listType));
            var newListExpression = ObjectCreationExpression(genericList).WithArgumentList(ArgumentList());
            var variableDeclaration = VariableDeclaration(genericList)
                    .WithVariables(SingletonSeparatedList(VariableDeclarator(Identifier(variableName)).WithInitializer(EqualsValueClause(newListExpression))));
            return LocalDeclarationStatement(variableDeclaration);
        }

        /// <summary>
        /// objectName.MethodName(arguments[])
        /// </summary>
        public static InvocationExpressionSyntax CallMethodOnObject(string objectName, string methodName, params ArgumentSyntax[] arguments)
        {
            var argumentList = ArgList(arguments);
            return InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,IdentifierName(objectName),IdentifierName(methodName)))
                         .WithArgumentList(argumentList);
        }

        /// <summary>
        /// (expression).MethodName(arguments[])
        /// </summary>
        public static ExpressionSyntax CallMethodOnExpr(this ExpressionSyntax expression, string methodName, params ArgumentSyntax[] arguments)
        {
            var argumentList = ArgList(arguments);
            if (arguments.Length == 0)
                return InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, expression, IdentifierName(methodName)));
            else
                return InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, expression, IdentifierName(methodName)))
                             .WithArgumentList(argumentList);
        }


        /// <summary>
        /// MethodName(arguments[])
        /// </summary>
        public static ExpressionSyntax CallMethod(string methodName, params ArgumentSyntax[] arguments)
        {
            if (arguments.Length == 0)
                return InvocationExpression(IdentifierName(methodName));
            else
            {
                var argumentList = ArgList(arguments);
                return InvocationExpression(IdentifierName(methodName)).WithArgumentList(argumentList);
            }
        }

        /// <summary>
        /// variableType variableName = expressionSyntax;
        /// </summary>
        public static VariableDeclarationSyntax AssignToNewVariable(this ExpressionSyntax expressionSyntax, TypeSyntax variableType, string variableName)
        {
            return VariableDeclaration(variableType).WithVariables(SingletonSeparatedList(VariableDeclarator(Identifier(variableName)).WithInitializer(EqualsValueClause(expressionSyntax))));
        }



        public static LocalDeclarationStatementSyntax ToStatement(this VariableDeclarationSyntax expression) => LocalDeclarationStatement(expression);
        public static ExpressionStatementSyntax ToStatement(this InvocationExpressionSyntax expression) => ExpressionStatement(expression);

        public static PrefixUnaryExpressionSyntax NotOnExpr(this ExpressionSyntax expression)
        {
            return PrefixUnaryExpression(SyntaxKind.LogicalNotExpression, expression);
        }

        /// <summary>
        /// An expression for x => x.ValueAccess
        /// </summary>
        public static SimpleLambdaExpressionSyntax LambdaAccessExpr(string x, string AccessProperty)
        {
            return SimpleLambdaExpression(Parameter(Identifier(x)))
                .WithExpressionBody(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(x), IdentifierName(AccessProperty)));
        }

        /// <summary>
        /// An expression for x => x.ValueAccess
        /// </summary>
        public static SimpleLambdaExpressionSyntax LambdaExpr(string itemName, ExpressionSyntax expressionWithItem)
        {
            return SimpleLambdaExpression(Parameter(Identifier(itemName))).WithExpressionBody(expressionWithItem);
        }

        public static void AddCommentToEndOfLine(this List<StatementSyntax> statements, string comment)
        {
            statements[statements.Count - 1] = statements[statements.Count - 1].WithTrailingTrivia(CommentTriv(comment));
        }

        public static void AddCommentToBeginning(this List<StatementSyntax> statements, string comment)
        {
            statements[0] = statements[0].WithLeadingTrivia(CommentTriv(comment));
        }

        public static AttributeListSyntax AttributeWithArgs(string attributeName, params AttributeArgumentSyntax[] args)
        {
            return AttributeList(SingletonSeparatedList(Attribute(IdentifierName(attributeName)).WithArgumentList(AttrArgList(args))));
        }

        /// <summary>
        /// Given a namespace in the form Namespace.Subnamespace etc... converts into a Namespace declaration!
        /// </summary>
        /// <param name="nomespace"></param>
        /// <returns></returns>
        public static NamespaceDeclarationSyntax GetNamespaceFromString(string nomespace)
        {
            var parts = nomespace.Split('.');
            return NamespaceDeclaration(AutoChainedName(parts));
        }
    }
}
