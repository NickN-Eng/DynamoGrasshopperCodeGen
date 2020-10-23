using DGCodeGen.Engine;
using System;

namespace DGCodeGen.TypeConversions
{
    /// <summary>
    /// A type which converts a custom class to a Gh_Goo<DGComTyp>.
    /// The custom class can is used directly within Dynamo.
    /// This assumes the Gh_Goo has a constructor of the form: Gh_Goo(DGComTyp)
    /// </summary>
    /// <typeparam name="DGComType"></typeparam>
    /// <typeparam name="GhParam"></typeparam>
    public class DataclassConversion : TypeConversion
    {
        public DataclassConversion(Type dataclassType)
        {
            _DGCommonType = dataclassType;
            _GrasshopperGooTypeName = _DGCommonType.Name + "_Goo";
            _GrasshopperParamTypeName = _DGCommonType.Name + "_Param";
        }

        private readonly Type _DGCommonType;
        private readonly string _GrasshopperGooTypeName;
        private readonly string _GrasshopperParamTypeName;

        public override Type DGCommonType => _DGCommonType;
        public override string GrasshopperTypeName => _GrasshopperGooTypeName;
        public override string DynamoTypeName => _DGCommonType.Name;

        public override SingleLineEvaluationStatements ConversionCode_DGCommonToGh(string variableName)
        {
            return SingleLineEvaluationStatements.FromCodeExpression($"new {_GrasshopperGooTypeName}({variableName})", _GrasshopperGooTypeName);
        }

        public override SingleLineEvaluationStatements ConversionCode_DGCommonToDy(string variableName) => null;

        public override SingleLineEvaluationStatements ConversionCode_GhToDGCommon(string variableName)
        {
            return SingleLineEvaluationStatements.FromCodeExpression($"{variableName}.Value", DGCommonType);
        }

        public override SingleLineEvaluationStatements ConversionCode_DyToDGCommon(string variableName) => null;

        public override SingleLineEvaluationStatements Gh_GetInitialisationDefault()
        {
            return SingleLineEvaluationStatements.FromCodeExpression("null", GrasshopperTypeName);
        }

        public override string Gh_AddParameterMethodName => "AddParameter";

        public override string Gh_CustomParameterName => _GrasshopperParamTypeName;

        public override EvaluationStatements Gh_DataIsInvalidStatement(string variableName)
        {
            return SingleLineEvaluationStatements.FromCodeExpression($"{variableName}.Value == null", typeof(bool));
        }

    }
}
