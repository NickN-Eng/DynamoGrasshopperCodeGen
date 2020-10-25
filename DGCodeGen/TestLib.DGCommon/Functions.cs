using DGCodeGen.Attributes;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//FYI!!! The code scanner ignores namespaces!!
namespace TestLib.DGCommon
{
    /// <summary>
    /// Nodes are defined in static functions, where each method is a node.
    /// </summary>
    public class Functions
    {
        [DGFunc(null, "SV", "SomeDataTests")]
        [DGDescription("Some description")]
        public static void VoidReturnTest([DGInput("Value", "V", "Desc")] double value)
        {
            double x = value * 2;
        }

        [DGFunc("SplitValue", "SV", "SomeDataTests")]
        [DGDescription("Some description")]
        [DGOutput("FirstValue", "FV", "fv")]
        [DGOutput("SecondValue", "SV", "sv")]
        [GhGuid("9f922787-923e-4cc9-b9c2-7b1af53bf4a0")]
        public static (double firstHalf, double secondHalf) SplitValue(
            [DGInput("Value", "V", "Desc")] double value
            )
        {
            double halfValue = value / 2;
            return (halfValue, halfValue);
        }

        [DGFunc("CreateSomeData", "CSD", "SomeDataTests")]
        [DGDescription("Some description")]
        [DGOutput("SomeData", "SD", "Description")]
        [GhGuid("ad7c0bdf-32f9-4be3-80ae-1bc44a7d9641")]
        public static SomeData CreateSomeData([DGInput("Integer", "I", "Desc")] int intValue)
        {
            var newSomeData = new SomeData(intValue);
            return newSomeData;
        }


        [DGFunc("CreateSomeListData", "CSD", "SomeDataTests")]
        [DGDescription("Some description")]
        [DGOutput("SomeDataList", "SD", "")]
        [GhGuid("4a774ae2-cb29-4f54-976d-05b0598d1484")]
        public static List<SomeData> CreateSomeListData(
            [DGInput("IntegerList", "I", "Desc")] List<int> intList
            )
        {
            var newSomeData = intList.Select(i => new SomeData(i)).ToList();
            return newSomeData;
        }

        [DGFunc("MultireturnTest", "CSD", "SomeDataTests")]
        [DGDescription("Some description")]
        [DGOutput("Value1", "SD", "")]
        [DGOutput("Value2", "SD", "")]
        [DGOutput("SomeDataList", "SD", "")]
        [GhGuid("2fb86028-3365-44b5-8da4-e042bb231502")]
        public static (int value1, int value2, List<SomeData> someDatas)
            MultireturnTest(
            [DGInput("IntegerList", "I", "Desc")] List<int> intList)
        {
            var newSomeData = intList.Select(i => new SomeData(i)).ToList();
            return (1, 2, newSomeData);
        }

        [GhFunc("CreatePointTest", "CPT", "GeometryTests")]
        [DGDescription("Some description")]
        [DGOutput("Point", "Pt", "Description")]
        public static Point3d CreatePointTest([DGInput("XYZ value", "XYZ", "Desc")] double value)
        {
            return new Point3d(value, value, value);
        }
    }

}