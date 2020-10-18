//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Grasshopper.Kernel;
//using Rhino.Geometry;

//// In order to load the result of this wizard, you will also need to
//// add the output bin/ folder of this project to the list of loaded
//// folder in Grasshopper.
//// You can use the _GrasshopperDeveloperSettings Rhino command for that.

//namespace TestLib.Grasshopper
//{
//    public class TestLibGrasshopperComponent : GH_Component
//    {
//        /// <summary>
//        /// Each implementation of GH_Component must provide a public 
//        /// constructor without any arguments.
//        /// Category represents the Tab in which the component will appear, 
//        /// Subcategory the panel. If you use non-existing tab or panel names, 
//        /// new tabs/panels will automatically be created.
//        /// </summary>
//        public TestLibGrasshopperComponent()
//          : base("CreateSomeData", "CSD", "Description", "Category", "Subcategory")
//        {
//        }

//        /// <summary>
//        /// Registers all the input parameters for this component.
//        /// </summary>
//        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
//        {
//            pManager.AddIntegerParameter("Int", "I", "Description", GH_ParamAccess.item);
//            pManager.AddParameter(new SomeData_Param(), "Some data", "SDx", "Some description", GH_ParamAccess.list);

//        }

//        /// <summary>
//        /// Registers all the output parameters for this component.
//        /// </summary>
//        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
//        {
//            pManager.AddParameter(new SomeData_Param(), "Some data", "SDx", "Some description", GH_ParamAccess.item);
//        }

//        /// <summary>
//        /// This is the method that actually does the work.
//        /// </summary>
//        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
//        /// to store data in output parameters.</param>
//        protected override void SolveInstance(IGH_DataAccess DA)
//        {
//            //Get inputs
//            int i = 0;
//            if (!DA.GetData(0, ref i)) return;

//            List<SomeData_Goo> someDatasInp_gh = new List<SomeData_Goo>();
//            if (!DA.GetDataList(1, someDatasInp_gh)) return;
//            List<SomeData> someDatasInp = new List<SomeData>();
//            foreach (var item in someDatasInp_gh)
//            {
//                var convertedItem = item.Value;
//                someDatasInp.Add(convertedItem);
//            }
//            //var someDatasInp = someDatasInp_gh.Select(sd_goo => sd_goo.Value).ToList();

//            //Function contents
//            var value = someDatasInp.Max(sd => sd.IntValue);
//            var someData_result = new SomeData(i + value);

//            //Output
//            var someData_gh = new SomeData_Goo(someData_result);
//            DA.SetData(0, someData_gh);
//        }

//        /// <summary>
//        /// Provides an Icon for every component that will be visible in the User Interface.
//        /// Icons need to be 24x24 pixels.
//        /// </summary>
//        protected override System.Drawing.Bitmap Icon
//        {
//            get
//            {
//                // You can add image files to your project resources and access them like this:
//                //return Resources.IconForThisComponent;
//                return null;
//            }
//        }

//        /// <summary>
//        /// Each component must have a unique Guid to identify it. 
//        /// It is vital this Guid doesn't change otherwise old ghx files 
//        /// that use the old ID will partially fail during loading.
//        /// </summary>
//        public override Guid ComponentGuid
//        {
//            get { return new Guid("d54525e5-ffaf-444f-b47d-dcd5ce021956"); }
//        }
//    }
//}
