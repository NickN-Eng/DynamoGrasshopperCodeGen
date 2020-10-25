using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace TestLib.Grasshopper
{
    public class CreatePointTest_Component : GH_Component
    {
        public CreatePointTest_Component(): base("CreatePointTest", "CPT", "Some description", "TestLib", "GeometryTests")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override System.Drawing.Bitmap Icon => null;
        public override Guid ComponentGuid => new Guid("9e1c98bf-e0d8-47b9-b47d-9894c4703035");
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("XYZ value", "XYZ", "Desc", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("Point", "Pt", "Description", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //Fetching node input: value.
            Double value = 0;
            if (!DA.GetData(0, ref value))
                return;
            //Function body
            Point3d result = new Point3d(value, value, value);
            //Setting node output: result.
            DA.SetData(0, result);
        }
    }
}