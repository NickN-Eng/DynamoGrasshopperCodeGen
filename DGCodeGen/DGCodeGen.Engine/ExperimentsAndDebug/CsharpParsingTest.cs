using Microsoft.CodeAnalysis.CSharp.Syntax;


namespace DGCodeGen.Engine
{
    public static class CsharpParsingTest
    {
        public static void Test()
        {
            string param = "coco";
            string expression = $"{param} + \"hihi\"";

            ExpressionSyntax expr1 = SyntaxHelper.GetExpression(expression);
            ExpressionSyntax expr2 = SyntaxHelper.GetExpression("SomeFunction(Hihi)");
            ExpressionSyntax expr3 = SyntaxHelper.GetExpression("SomeFunction(\"Hihih\", 1)");
            ExpressionSyntax expr4 = SyntaxHelper.GetExpression("(1,\"hihi\")");
            ExpressionSyntax expr5 = SyntaxHelper.GetExpression("SomeObj.ToString()");
            ExpressionSyntax expr6 = SyntaxHelper.GetExpression("1+1");
            ExpressionSyntax expr7 = SyntaxHelper.GetExpression("");
            //var tree = CSharpSyntaxTree.ParseText(expression, new CSharpParseOptions(kind: Microsoft.CodeAnalysis.SourceCodeKind.Script));
            //var root = tree.GetRoot();
            //var descendants = root.DescendantNodes().ToList();
        }
    }
}
