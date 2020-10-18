using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;


namespace DGCodeGen.Engine
{
    /// <summary>
    /// An evaluation statement which wraps an arrow expression or single line code expression.
    /// This code logic can then be inserted into a syntax tree as follows:
    ///  - CreateStatementsAsVariable(var variableName)
    ///  - CreateStatementsAsMethod
    /// </summary>
    public class SingleLineEvaluationStatements : EvaluationStatements
    {
        public ExpressionSyntax ExpressionNode;

        public TypeSyntax ReturnTypeSyntax; //The priority return type
        public Type ReturnTypeRefl;

        public override bool CanCreateStatementsAsVariable => true;

        public override bool IsTupleReturn
        {
            get
            {
                if (ReturnTypeSyntax != null)
                    return ReturnTypeSyntax is TupleTypeSyntax;
                else
                    return ReflectionHelper.IsValueTuple(ReturnTypeRefl);
            }
        }

        public override TypeSyntax ReturnType => ReturnTypeSyntax != null ? ReturnTypeSyntax : IdentifierName(ReturnTypeRefl.Name);

        //public string ReturnTypeFullName;

        public SingleLineEvaluationStatements(ExpressionSyntax expressionNode, TypeSyntax returnTypeSyntax)
        {
            ExpressionNode = expressionNode;
            ReturnTypeSyntax = returnTypeSyntax;
        }

        public SingleLineEvaluationStatements(ExpressionSyntax expressionNode, Type returnType)
        {
            ExpressionNode = expressionNode;
            ReturnTypeRefl = returnType;
        }

        /// <summary>
        /// Parse C# code which is a single line expression which evaluates to the provided type.
        /// The code exclude the semi colon.
        /// </summary>
        public static SingleLineEvaluationStatements FromCodeExpression(string codeAsExpression, Type returnType)
        {
            var tree = CSharpSyntaxTree.ParseText(codeAsExpression, new CSharpParseOptions(kind: SourceCodeKind.Script));
            //var debugNodeList = tree.GetRoot().DescendantNodes();
            var expressionNode = tree.GetRoot().DescendantNodes().FirstOrDefault(node => node is ExpressionSyntax);
            if (expressionNode == null)
                throw new Exception("Expression string does not contain an expression syntax node. Sytax contained is "
                    + string.Join(",", tree.GetRoot().ChildNodes().Select(node => node.Kind().ToString()).ToArray()));

            var singleExpression = new SingleLineEvaluationStatements((ExpressionSyntax)expressionNode, returnType);
            return singleExpression;
        }

        public static SingleLineEvaluationStatements FromArrowExpressionMethod(MethodDeclarationSyntax methodSyntax)
        {
            if (methodSyntax.ExpressionBody == null) throw new Exception("Single expression must have an expression body.");

            var singleExpression = new SingleLineEvaluationStatements(methodSyntax.ExpressionBody.Expression, methodSyntax.ReturnType);
            //singleExpression.ReturnTypeSyntax = methodSyntax.ReturnType;
            return singleExpression;
        }

        public override List<StatementSyntax> CreateStatementsAsVariable(string variableName)
        {
            //var obj = Functiony(1, 1);
            var statement = LocalDeclarationStatement(VariableDeclaration(ReturnType)
                                .WithVariables(SingletonSeparatedList(VariableDeclarator(Identifier(variableName))
                                .WithInitializer(EqualsValueClause(ExpressionNode)))));
            return new List<StatementSyntax>() { statement };
        }

        public override ExpressionSyntax CreateStatementsAsExpression()
        {
            return ExpressionNode;
        }

        public override MethodDeclarationSyntax CreateStatementsAsMethod(string methodName, params SyntaxKind[] accessModifiers)
        {
            return MethodDeclaration(ReturnTypeSyntax, methodName)
                .WithModifiers(TokenList(accessModifiers.Select(synKind => Token(synKind))))
                //.WithParameterList()
                .WithExpressionBody(ArrowExpressionClause(ExpressionNode))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
        }


        public override List<StatementSyntax> CreateStatementsAsTupleVariables(string[] variableNames)
        {
            if (!(ExpressionNode is TupleExpressionSyntax tupleExpression))
                throw new Exception("Requesting tuple statements. Should have a tuple expression syntax");

            TypeSyntax[] tupleTypes;
            if(ReturnTypeSyntax != null)
            {
                if (ReturnTypeSyntax is TupleTypeSyntax tupleTypeSyntax)
                    tupleTypes = tupleTypeSyntax.Elements.Select(tupElem => tupElem.Type).ToArray();
                else
                    throw new Exception("Requesting tuple statements. Should have a tuple type syntax");
            }
            else
            {
                if (ReflectionHelper.IsValueTuple(ReturnTypeRefl))
                    tupleTypes = ReturnTypeRefl.GetTypeInfo().GetFields().Select(feild => IdentifierName(feild.FieldType.Name)).ToArray();
                else
                    throw new Exception("Requesting tuple statements. Should have a tuple type.");
            }

            if (tupleTypes.Length != variableNames.Length)
                throw new Exception("Return type list should have same length as supplied variable names.");

            var statements = new List<StatementSyntax>();
            int varNo = 0;
            foreach (ArgumentSyntax arg in tupleExpression.Arguments)
            {
                var statement = LocalDeclarationStatement(VariableDeclaration(tupleTypes[varNo])
                    .WithVariables(SingletonSeparatedList(VariableDeclarator(Identifier(variableNames[varNo]))
                    .WithInitializer(EqualsValueClause(arg.Expression)))));
                statements.Add(statement);
                varNo++;
            }
            return statements;
        }


    }
}
