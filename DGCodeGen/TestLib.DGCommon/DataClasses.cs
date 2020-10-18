using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DGCodeGen.Attributes;

namespace TestLib.DGCommon
{
    /// <summary>
    /// This is a dataclass which will by picked up by DGCodeGen. 
    /// It can be written into grasshopper as a class which is wrapped as an Goo<Class> => Param<Goo<Class>>
    /// </summary>
    [GhDataClass("SD", "TestLib", "SomeDataTests")]
    [DGDescription("Some data data data.")]
    [GhGuid("f932581a-ab25-4b4e-9665-2a27f078673a")]
    public class SomeData
    {
        public int IntValue;

        public SomeData(int intValue)
        {
            IntValue = intValue;
        }

        public SomeData(SomeData someData)
        {
            IntValue = someData.IntValue;
        }

        public override string ToString()
        {
            return $"{typeof(SomeData)} Value={IntValue}";
        }
    }
}
