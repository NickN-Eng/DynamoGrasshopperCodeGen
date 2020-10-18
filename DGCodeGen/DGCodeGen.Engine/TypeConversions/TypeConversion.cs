using System;
using DGCodeGen.Engine;

namespace DGCodeGen.TypeConversions
{
    public abstract class TypeConversion
    {
        /// <summary> The type used in the DGCommon function definition.</summary>
        public abstract Type DGCommonType { get; }

        /// <summary>
        /// Type which is used in the Grasshopper library. 
        /// Return null if this type CANNOT be used in grasshopper
        /// </summary>
        public abstract Type GrasshopperType { get; }

        /// <summary>
        /// Type which is used in the Dynamo library. 
        /// Return null if this type CANNOT be used in Dynamo
        /// </summary>
        public abstract Type DynamoType { get; }

        /// <summary>
        /// Code to convert from the DGCommon type to a Grasshopper type.
        /// Return null if a conversion is NOT REQUIRED.
        /// </summary>
        /// <param name="variableName">The name of the DGCommonType variable to be included in the evaluation expression </param>
        public abstract SingleLineEvaluationStatements ConversionCode_DGCommonToGh(string variableName);

        /// <summary>
        /// Code to convert from the DGCommon type to a Dynamo type.
        /// Return null if a conversion is NOT REQUIRED.
        /// </summary>
        /// <param name="variableName">The name of the DGCommonType variable to be included in the evaluation expression </param>
        public abstract SingleLineEvaluationStatements ConversionCode_DGCommonToDy(string variableName);

        /// <summary>
        /// Code to convert from the Grasshopper type to a DGCommon type.
        /// Return null if a conversion is NOT REQUIRED.
        /// </summary>
        /// <param name="variableName">The name of the Grasshopper variable to be included in the evaluation expression </param>
        public abstract SingleLineEvaluationStatements ConversionCode_GhToDGCommon(string variableName);

        /// <summary>
        /// Code to convert from the Dynamo type to a DGCommon type.
        /// Return null if a conversion is NOT REQUIRED.
        /// </summary>
        /// <param name="variableName">The name of the Dynamo variable to be included in the evaluation expression </param>
        public abstract SingleLineEvaluationStatements ConversionCode_DyToDGCommon(string variableName);

        /// <summary>
        /// Sets the empty variable to which this class can be initialised, before calling DA.GetData(x, ref variable)...
        /// MUST be a self contained expression with no input parameters.
        /// E.g.:   Rhino.Geometry.Circle circle = Rhino.Geometry.Circle.Unset;
        ///         if (!DA.GetData(0, ref circle)) { return; }
        /// The InitialisationDefault from above is "Rhino.Geometry.Circle.Unset".
        /// </summary>
        public abstract SingleLineEvaluationStatements Gh_GetInitialisationDefault();

        /// <summary>
        /// A state to check if the variable of this type is valid.
        /// This is an expression that returns true if the variable is Invalid e.g. !circle.IsValid
        /// </summary>
        public abstract EvaluationStatements Gh_DataIsInvalidStatement(string variableName);

        /// <summary>
        /// The name of the method to add onto the grasshopper ParameterManager.
        /// E.g. pManager.AddLineParameter("Line", "L", "Slicing line", GH_ParamAccess.item);  
        /// Should be of the form: "AddXXXParameter"
        /// </summary>
        public abstract string Gh_AddParameterMethodName { get; }

        /// <summary>
        /// The parameter name for this GhGoo type. Only required for custom parameters.
        /// If this GhGoo has a predefined parameter (e.g. Integer), the parameter type is not required, return null.
        /// </summary>
        public abstract string Gh_CustomParameterName { get; }
    }

    public abstract class TypeConversionBase<DGComType, GhType, DyType> : TypeConversion
    {
        public override Type DGCommonType => typeof(DGComType);
        public override Type GrasshopperType => typeof(GhType);
        public override Type DynamoType => typeof(DyType);
    }

    public abstract class DynamoOnlyTypeConversion<DyType> : TypeConversion
    {
        public override Type DGCommonType => typeof(DyType);

        public override Type GrasshopperType => null;

        public override Type DynamoType => typeof(DyType);

        public override SingleLineEvaluationStatements ConversionCode_DGCommonToDy(string variableName) => null;

        public override SingleLineEvaluationStatements ConversionCode_DGCommonToGh(string variableName) => throw new NotImplementedException();

        public override SingleLineEvaluationStatements ConversionCode_DyToDGCommon(string variableName) => null;

        public override SingleLineEvaluationStatements ConversionCode_GhToDGCommon(string variableName) => throw new NotImplementedException();

        public override string Gh_AddParameterMethodName => throw new NotImplementedException();

        public override string Gh_CustomParameterName => throw new NotImplementedException();

        public override EvaluationStatements Gh_DataIsInvalidStatement(string variableName) => throw new NotImplementedException();

        public override SingleLineEvaluationStatements Gh_GetInitialisationDefault() => throw new NotImplementedException();
    }
}
