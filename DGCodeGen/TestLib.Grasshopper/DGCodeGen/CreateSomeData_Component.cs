using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace TestLib.Grasshopper
{
    public class CreateSomeData_Component : GH_Component
    {
        public CreateSomeData_Component(): base("CreateSomeData", "CSD", "Some description", "TestLib", "SomeDataTests")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override System.Drawing.Bitmap Icon => null;
        public override Guid ComponentGuid => new Guid("ad7c0bdf-32f9-4be3-80ae-1bc44a7d9641");
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("Integer", "I", "Desc", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new SomeData_Param(), "SomeData", "SD", "", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //Fetching node input: intValue.
            Int32 intValue = 0;
            if (!DA.GetData(0, ref intValue))
                return;
            //Function body
            var newSomeData = new SomeData(intValue);
            SomeData result = newSomeData;
            //Setting node output: result.
            SomeData_Goo result_gh = new SomeData_Goo(result);
            DA.SetData(0, result_gh);
        }
    }
}