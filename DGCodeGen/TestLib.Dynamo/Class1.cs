using Autodesk.DesignScript.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestLib.Dynamo
{
    public static class Class1
    {
        [MultiReturn("thing 1", "thing 2")]
        public static Dictionary<string, object> MultiReturnExample()
        {
            return new Dictionary<string, object>()
            {
                { "thing 1", new List<string>{"apple", "banana", "cat"} },
                { "thing 2", new List<string>{"Tywin", "Cersei", "Hodor"} }
            };
        }
    }
}
