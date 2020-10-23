using Rhino.Geometry;
using System;
using DGCodeGen.Engine;

namespace DGCodeGen.TypeConversions
{
    /// <summary>
    /// A type converter which only has an implementation for Grasshopper types.
    /// </summary>
    /// <typeparam name="GhType"></typeparam>
    public abstract class GrasshopperOnlyConversion<GhType> : TypeConversion
    {
        public override Type DGCommonType => typeof(GhType);

        public override string GrasshopperTypeName => GrasshopperType.Name;
        public override string DynamoTypeName => null;
        public Type GrasshopperType => typeof(GhType);

        public override SingleLineEvaluationStatements ConversionCode_DGCommonToDy(string variableName) => throw new NotImplementedException();

        public override SingleLineEvaluationStatements ConversionCode_DGCommonToGh(string variableName) => null;

        public override SingleLineEvaluationStatements ConversionCode_DyToDGCommon(string variableName) => throw new NotImplementedException();

        public override SingleLineEvaluationStatements ConversionCode_GhToDGCommon(string variableName) => null;
    }

    public abstract class GrasshopperOnlyPredefStructConversion<GhType> : GrasshopperOnlyConversion<GhType>
    {
        public override string Gh_CustomParameterName => null;

        public override EvaluationStatements Gh_DataIsInvalidStatement(string variableName) => SingleLineEvaluationStatements.FromCodeExpression($"{variableName}.IsValid", typeof(GhType));

        public override SingleLineEvaluationStatements Gh_GetInitialisationDefault() => SingleLineEvaluationStatements.FromCodeExpression(Gh_InitialisationDefaultCode, typeof(GhType));

        public abstract string Gh_InitialisationDefaultCode { get; }
    }

    public abstract class GrasshopperOnlyPredefClassConversion<GhType> : GrasshopperOnlyConversion<GhType>
    {
        public override string Gh_CustomParameterName => null;

        public override EvaluationStatements Gh_DataIsInvalidStatement(string variableName) => SingleLineEvaluationStatements.FromCodeExpression($"{variableName}.IsValid", typeof(GhType));

        public override SingleLineEvaluationStatements Gh_GetInitialisationDefault() => SingleLineEvaluationStatements.FromCodeExpression("null", typeof(GhType));
    }

    public static class TestClass
    {
        public static void TestCode()
        {
            var point = Point3d.Origin;
            bool pointValid = point.IsValid;

            var vector = Vector3d.Zero;
            bool vectorValid = vector.IsValid;

            var arc = Arc.Unset;
            bool arcValid = arc.IsValid;

            var box = Box.Unset;
            bool boxValid = box.IsValid;

            Brep brep = null;
            bool brepValid = brep.IsValid;

            Curve curve = null;
            bool curveValid = curve.IsValid;

            Line line = Line.Unset;
            bool lineValid = line.IsValid;

            Plane plane = Plane.Unset;
            bool planeValid = plane.IsValid;
        }
    }

    public class GrasshopperPoint3dConversion : GrasshopperOnlyPredefStructConversion<Point3d>
    {
        public override string Gh_AddParameterMethodName => "AddPointParameter";

        public override string Gh_InitialisationDefaultCode => "Point3d.Origin";
    }

    public class GrasshopperVector3dConversion : GrasshopperOnlyPredefStructConversion<Vector3d>
    {
        public override string Gh_AddParameterMethodName => "AddVectorParameter";

        public override string Gh_InitialisationDefaultCode => "Vector3d.Zero";
    }

    public class GrasshopperArcConversion : GrasshopperOnlyPredefStructConversion<Arc>
    {
        public override string Gh_AddParameterMethodName => "AddArcParameter";

        public override string Gh_InitialisationDefaultCode => "Arc.Unset";
    }

    public class GrasshopperBoxConversion : GrasshopperOnlyPredefStructConversion<Box>
    {
        public override string Gh_AddParameterMethodName => "AddBoxParameter";

        public override string Gh_InitialisationDefaultCode => "Box.Unset";
    }

    public class GrasshopperLineConversion : GrasshopperOnlyPredefStructConversion<Line>
    {
        public override string Gh_AddParameterMethodName => "AddLineParameter";

        public override string Gh_InitialisationDefaultCode => "Line.Unset";
    }

    public class GrasshopperPlaneConversion : GrasshopperOnlyPredefStructConversion<Plane>
    {
        public override string Gh_AddParameterMethodName => "AddPlaneParameter";

        public override string Gh_InitialisationDefaultCode => "Plane.Unset";
    }
}
