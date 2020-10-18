using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Microsoft.CodeAnalysis.CSharp;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using DGCodeGen.Engine;
namespace DGCodeGen.TypeConversions
{
    /// <summary>
    /// A type which converts a custom class to a Gh_Goo<DGComTyp>.
    /// The custom class can is used directly within Dynamo.
    /// This assumes the Gh_Goo has a constructor of the form: Gh_Goo(DGComTyp)
    /// </summary>
    /// <typeparam name="DGComType"></typeparam>
    /// <typeparam name="GhParam"></typeparam>
    public abstract class NoConversionTypeConversion<DGComType> : TypeConversionBase<DGComType, DGComType, DGComType>
    {
        public override SingleLineEvaluationStatements ConversionCode_DGCommonToGh(string variableName) => null;

        public override SingleLineEvaluationStatements ConversionCode_DGCommonToDy(string variableName) => null;

        public override SingleLineEvaluationStatements ConversionCode_GhToDGCommon(string variableName) => null;

        public override SingleLineEvaluationStatements ConversionCode_DyToDGCommon(string variableName) => null;

    }

    public class IntegerTypeConversion : NoConversionTypeConversion<int>//, GH_Integer>
    {
        public override SingleLineEvaluationStatements Gh_GetInitialisationDefault()
        {
            return new SingleLineEvaluationStatements(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(0)), GrasshopperType);
        }
         
        public override string Gh_AddParameterMethodName => "AddIntegerParameter";

        public override string Gh_CustomParameterName => null;

        public override EvaluationStatements Gh_DataIsInvalidStatement(string variableName) => null;
    }

    public class DoubleTypeConversion : NoConversionTypeConversion<double>//, GH_Number>
    {
        public override SingleLineEvaluationStatements Gh_GetInitialisationDefault()
        {
            return new SingleLineEvaluationStatements(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(0)), GrasshopperType);
        }

        public override string Gh_AddParameterMethodName => "AddNumberParameter";
        public override string Gh_CustomParameterName => null;

        public override EvaluationStatements Gh_DataIsInvalidStatement(string variableName) => null;
    }

    public class BooleanTypeConversion : NoConversionTypeConversion<bool>//, GH_Boolean>
    {
        public override SingleLineEvaluationStatements Gh_GetInitialisationDefault()
        {
            return new SingleLineEvaluationStatements(LiteralExpression(SyntaxKind.TrueKeyword), GrasshopperType);
        }

        public override string Gh_AddParameterMethodName => "AddBooleanParameter";
        public override string Gh_CustomParameterName => null;

        public override EvaluationStatements Gh_DataIsInvalidStatement(string variableName) => null;
    }

    public class StringTypeConversion : NoConversionTypeConversion<string>//, GH_String>
    {
        public override SingleLineEvaluationStatements Gh_GetInitialisationDefault()
        {
            return new SingleLineEvaluationStatements(LiteralExpression(SyntaxKind.StringLiteralExpression,Literal("")), GrasshopperType);
        }

        public override string Gh_AddParameterMethodName => "AddTextParameter";
        public override string Gh_CustomParameterName => null;

        public override EvaluationStatements Gh_DataIsInvalidStatement(string variableName) => null;
    }
}
