using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.DesignScript.Runtime;

namespace TestLib.Dynamo
{
    public static class SomeDataTests
    {
        [MultiReturn("FirstValue", "SecondValue")]
        public static Dictionary<string, object> SplitValue(Double Value)
        {
            var value = Value;
            double halfValue = value / 2;
            double Item1 = halfValue;
            double Item2 = halfValue;
            return new Dictionary<string, object>()
            {{"FirstValue", Item1}, {"SecondValue", Item2}};
        }

        public static SomeData CreateSomeData(Int32 Integer)
        {
            var intValue = Integer;
            var newSomeData = new SomeData(intValue);
            SomeData result = newSomeData;
            return result;
        }

        public static SomeData CreateSomeListData(Int32 IntegerList)
        {
            var intList = IntegerList;
            var newSomeData = intList.Select(i => new SomeData(i)).ToList();
            List<SomeData> result = newSomeData;
            return result;
        }

        [MultiReturn("Value1", "Value2", "SomeDataList")]
        public static Dictionary<string, object> MultireturnTest(Int32 IntegerList)
        {
            var intList = IntegerList;
            var newSomeData = intList.Select(i => new SomeData(i)).ToList();
            int Item1 = 1;
            int Item2 = 2;
            List<SomeData> Item3 = newSomeData;
            return new Dictionary<string, object>()
            {{"Value1", Item1}, {"Value2", Item2}, {"SomeDataList", Item3}};
        }
    }
}