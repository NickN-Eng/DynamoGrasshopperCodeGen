using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;


namespace DGCodeGen.Engine
{
    /// <summary>
    /// A class which can contains code to return an evaluated expression.
    /// It can parse either an arrow expression, or a method block.
    /// From this, it can insert the logic into output code, through a number of ways:
    ///  - CreateStatementsAsVariable(var variableName)
    ///  - CreateStatementsAsMethod something
    ///  This class does NOT contain the method names etc... this is provided by CreateStatementAsMethod()
    /// </summary>
    public abstract class EvaluationStatements
    {
        public abstract MethodDeclarationSyntax CreateStatementsAsMethod(string methodName, params SyntaxKind[] accessModifiers);

        public abstract TypeSyntax ReturnType { get; }

        /// <summary>
        /// Returns a list of statements, where the result of this evaluation statement is written as a variable declaration.
        /// E.g. Expression: "i + 1"  => Output statement: "result = i + 1;"
        /// </summary>
        /// <param name="variableName">The variable name for the variable declaration. I.e. for above example: "result".</param>
        public abstract List<StatementSyntax> CreateStatementsAsVariable(string variableName);

        /// <summary>
        /// Returns null if this is not possible
        /// </summary>
        public abstract ExpressionSyntax CreateStatementsAsExpression();
        public abstract List<StatementSyntax> CreateStatementsAsTupleVariables(string[] variableNames);

        public abstract List<StatementSyntax> CreateRawStatements();


        public static EvaluationStatements FromMethod(MethodDeclarationSyntax methodDeclarationSyntax)
        {
            if (methodDeclarationSyntax.Body == null)
                return SingleLineEvaluationStatements.FromArrowExpressionMethod(methodDeclarationSyntax);
            else
                return BodyBlockEvaluationStatements.FromMethodWithBody(methodDeclarationSyntax);
        }

        /// <summary>
        /// True if this statement needs to be declared in a private method, rather than inserted into another method directly.
        /// Can only be inserted into the method if there is only ONE return statement, and it is the LAST statement in the body block.
        /// If these criteria are not met, this EvaluationStatement must be placed in a private method.
        /// </summary>
        public abstract bool CanCreateStatementsAsVariable { get; }

        public abstract bool IsTupleReturn { get; }
    }
}
