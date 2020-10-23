using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace TestLib.Grasshopper
{
    public class CreateSomeListData_Component : GH_Component
    {
        public CreateSomeListData_Component(): base("CreateSomeListData", "CSD", "Some description", "TestLib", "SomeDataTests")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override System.Drawing.Bitmap Icon => null;
        public override Guid ComponentGuid => new Guid("4a774ae2-cb29-4f54-976d-05b0598d1484");
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("IntegerList", "I", "Desc", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new SomeData_Param(), "SomeDataList", "SD", "", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //Fetching node input: intList.
            List<Int32> intList = new List<Int32>();
            if (!DA.GetDataList(0, intList))
                return;
            //Function body
            var newSomeData = intList.Select(i => new SomeData(i)).ToList();
            List<SomeData> result = newSomeData;
            //Setting node output: result.
            List<SomeData_Goo> result_gh = result.Select(item => new SomeData_Goo(item)).ToList();
            DA.SetDataList(0, result_gh);
        }
    }
}