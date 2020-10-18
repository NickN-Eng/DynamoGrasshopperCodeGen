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
    public abstract class CustomClassConversion<DGComType, GhGoo> : DyAndGhGooConversion<DGComType, GhGoo>
    {
        public override SingleLineEvaluationStatements Gh_GetInitialisationDefault()
        {
            return SingleLineEvaluationStatements.FromCodeExpression("null", typeof(GhGoo));
        }

        public override string Gh_AddParameterMethodName => "AddParameter";

        public override EvaluationStatements Gh_DataIsInvalidStatement(string variableName)
        {
            return SingleLineEvaluationStatements.FromCodeExpression($"{variableName}.Value == null", typeof(bool));
        }

    }
}
