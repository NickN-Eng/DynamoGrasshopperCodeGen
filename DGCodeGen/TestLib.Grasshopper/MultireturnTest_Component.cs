using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace TestLib.Grasshopper
{
    public class MultireturnTest_Component : GH_Component
    {
        public MultireturnTest_Component(): base("MultireturnTest", "CSD", "Some description", "TestLib", "SomeDataTests")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override System.Drawing.Bitmap Icon => null;
        public override Guid ComponentGuid => new Guid("2fb86028-3365-44b5-8da4-e042bb231502");
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("IntegerList", "I", "Desc", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddIntegerParameter("Value1", "SD", "", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Value2", "SD", "", GH_ParamAccess.item);
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
            int Item1 = 1;
            int Item2 = 2;
            List<SomeData> Item3 = newSomeData;
            //Setting node output: Item1.
            DA.SetData(0, Item1);
            //Setting node output: Item2.
            DA.SetData(1, Item2);
            //Setting node output: Item3.
            List<SomeData_Goo> Item3_gh = Item3.Select(item => new SomeData_Goo(item)).ToList();
            DA.SetDataList(2, Item3_gh);
        }
    }
}