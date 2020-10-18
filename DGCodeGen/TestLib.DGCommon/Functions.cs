using DGCodeGen.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestLib.Grasshopper;

//FYI!!! The code scanner ignores namespaces!!
namespace TestLib.DGCommon
{
    /// <summary>
    /// Nodes are defined in static functions, where each method is a node.
    /// </summary>
    public class Functions
    {
        [GhFunc("SplitValue", "SV", "TestLib", "SomeDataTests")]
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

        [GhFunc("CreateSomeData", "CSD", "TestLib", "SomeDataTests")]
        [DGDescription("Some description")]
        [DGOutput("SomeData", "SD", "")]
        [GhGuid("ad7c0bdf-32f9-4be3-80ae-1bc44a7d9641")]
        public static SomeData CreateSomeData(
            [DGInput("Integer", "I", "Desc")] int intValue
            )
        {
            var newSomeData = new SomeData(intValue);
            return newSomeData;
        }


        [GhFunc("CreateSomeListData", "CSD", "TestLib", "SomeDataTests")]
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

        [GhFunc("MultireturnTest", "CSD", "TestLib", "SomeDataTests")]
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
    }

}