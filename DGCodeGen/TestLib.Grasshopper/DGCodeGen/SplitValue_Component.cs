using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace TestLib.Grasshopper
{
    public class SplitValue_Component : GH_Component
    {
        public SplitValue_Component(): base("SplitValue", "SV", "Some description", "TestLib", "SomeDataTests")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override System.Drawing.Bitmap Icon => null;
        public override Guid ComponentGuid => new Guid("9f922787-923e-4cc9-b9c2-7b1af53bf4a0");
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Value", "V", "Desc", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("FirstValue", "FV", "fv", GH_ParamAccess.item);
            pManager.AddNumberParameter("SecondValue", "SV", "sv", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //Fetching node input: value.
            Double value = 0;
            if (!DA.GetData(0, ref value))
                return;
            //Function body
            double halfValue = value / 2;
            double Item1 = halfValue;
            double Item2 = halfValue;
            //Setting node output: Item1.
            DA.SetData(0, Item1);
            //Setting node output: Item2.
            DA.SetData(1, Item2);
        }
    }
}