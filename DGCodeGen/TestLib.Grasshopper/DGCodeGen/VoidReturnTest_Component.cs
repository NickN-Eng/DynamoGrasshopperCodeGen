using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace TestLib.Grasshopper
{
    public class VoidReturnTest_Component : GH_Component
    {
        public VoidReturnTest_Component(): base("VoidReturnTest", "SV", "Some description", "TestLib", "SomeDataTests")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override System.Drawing.Bitmap Icon => null;
        public override Guid ComponentGuid => new Guid("99d9ae9b-d538-45d7-be5d-6944c5308953");
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Value", "V", "Desc", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //Fetching node input: value.
            Double value = 0;
            if (!DA.GetData(0, ref value))
                return;
            //Function body
            double x = value * 2;
        }
    }
}