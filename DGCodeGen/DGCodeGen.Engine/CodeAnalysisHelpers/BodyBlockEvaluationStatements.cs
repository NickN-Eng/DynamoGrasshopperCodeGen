using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;


namespace DGCodeGen.Engine
{
    /// <summary>
    /// An evaluation statement which wraps a method body block,
    /// This code logic can then be inserted into a syntax tree as follows:
    ///  - CreateStatementsAsVariable(var variableName)
    ///  - CreateStatementsAsMethod
    /// </summary>
    public class BodyBlockEvaluationStatements : EvaluationStatements
    {
        //public MethodDeclarationSyntax Method; //Is this necessary??
        public BlockSyntax Body;

        private TypeSyntax _ReturnType;
        public override TypeSyntax ReturnType => _ReturnType;

        public BodyBlockEvaluationStatements(BlockSyntax body, TypeSyntax returnTypeSyntax)
        {
            Body = body;
            _ReturnType = returnTypeSyntax;

            int returnStatementCount = Body.DescendantNodes().Count(node => node is ReturnStatementSyntax);
            _CanCreateStatementsAsVariable = returnStatementCount == 1 && Body.Statements.Last() is ReturnStatementSyntax;
        }

        private bool _CanCreateStatementsAsVariable;

        public override bool CanCreateStatementsAsVariable => _CanCreateStatementsAsVariable;


        public override bool IsTupleReturn => ReturnType is TupleTypeSyntax;


        public static BodyBlockEvaluationStatements FromMethodWithBody(MethodDeclarationSyntax methodSyntax)
        {
            if (methodSyntax.Body == null) throw new Exception("Method body expression must have a Body Block.");

            var mbe = new BodyBlockEvaluationStatements
            (
                //Method = methodSyntax,
                body: methodSyntax.Body,
                returnTypeSyntax: methodSyntax.ReturnType
            );
            return mbe;
        }

        public override MethodDeclarationSyntax CreateStatementsAsMethod(string methodName, params SyntaxKind[] accessModifiers)
        {
            return MethodDeclaration(ReturnType, methodName)
                .WithModifiers(TokenList(accessModifiers.Select(synKind => Token(synKind))))
                .WithBody(Body)
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
        }

        public override List<StatementSyntax> CreateStatementsAsTupleVariables(string[] variableNames)
        {
            if (!_CanCreateStatementsAsVariable)
                throw new Exception("This statement cannot be created as a variable");

            //Get the return statment, and check that it is a TupleExpressionSyntax
            var returnStatement = (ReturnStatementSyntax)Body.Statements.Last();
            if (!(returnStatement.Expression is TupleExpressionSyntax tupleExpression))
                throw new Exception("Requesting tuple statements. Should have a tuple expression syntax");

            //Check that return type syntax is for a tuple, and split as an array
            TypeSyntax[] tupleTypes;
            if (ReturnType is TupleTypeSyntax tupleTypeSyntax)
                tupleTypes = tupleTypeSyntax.Elements.Select(tupElem => tupElem.Type).ToArray();
            else
                throw new Exception("Requesting tuple statements. Should have a tuple type syntax");

            //Check that the tuple return types list length matches the tuple expressions
            if (tupleTypes.Length != variableNames.Length)
                throw new Exception("Return type list should have same length as supplied variable names.");

            //Create a list of statements for the preceding statements
            var statements = new List<StatementSyntax>(Body.Statements.Take(Body.Statements.Count - 1));

            //Add each result statement
            int varNo = 0;
            foreach (ArgumentSyntax arg in tupleExpression.Arguments)
            {
                var resultStatement = LocalDeclarationStatement(VariableDeclaration(tupleTypes[varNo])
                    .WithVariables(SingletonSeparatedList(VariableDeclarator(Identifier(variableNames[varNo]))
                    .WithInitializer(EqualsValueClause(arg.Expression)))));
                statements.Add(resultStatement);
                varNo++;
            }

            return statements;
        }

        public override List<StatementSyntax> CreateStatementsAsVariable(string variableName)
        {
            if (!_CanCreateStatementsAsVariable)
                throw new Exception("This statement cannot be created as a variable");

            var statements = new List<StatementSyntax>(Body.Statements.Take(Body.Statements.Count - 1));
            var returnStatement = (ReturnStatementSyntax)Body.Statements.Last();

            var resultVariableStatement = LocalDeclarationStatement(VariableDeclaration(ReturnType)
                                .WithVariables(SingletonSeparatedList(VariableDeclarator(Identifier(variableName))
                                .WithInitializer(EqualsValueClause(returnStatement.Expression)))));
            statements.Add(resultVariableStatement);
            return statements;
        }

        public override ExpressionSyntax CreateStatementsAsExpression()
        {
            return null;
        }

        public override List<StatementSyntax> CreateRawStatements()
        {
            return Body.Statements.ToList();
        }
    }
}
