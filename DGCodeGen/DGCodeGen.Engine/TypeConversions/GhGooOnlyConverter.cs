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
    public abstract class DyAndGhGooConversion<DGComType, GhGoo> : TypeConversionBase<DGComType, GhGoo, DGComType>
    {
        public override SingleLineEvaluationStatements ConversionCode_DGCommonToGh(string variableName)
        {
            return SingleLineEvaluationStatements.FromCodeExpression($"new {GrasshopperTypeName}({variableName})", typeof(GhGoo));
        }

        public override SingleLineEvaluationStatements ConversionCode_DGCommonToDy(string variableName) => null;

        public override SingleLineEvaluationStatements ConversionCode_GhToDGCommon(string variableName)
        {
            return SingleLineEvaluationStatements.FromCodeExpression($"{variableName}.Value", typeof(DGComType));
        }

        public override SingleLineEvaluationStatements ConversionCode_DyToDGCommon(string variableName) => null;

    }
}
