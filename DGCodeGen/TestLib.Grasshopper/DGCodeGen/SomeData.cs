using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace TestLib.Grasshopper
{
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

    public class SomeData_Goo : GH_Goo<SomeData>
    {
        public override bool IsValid => true;
        public override string TypeName => "SomeData";
        public override string TypeDescription => "A bit of data.";
        public SomeData_Goo(SomeData someData)
        {
            Value = someData;
        }

        public override IGH_Goo Duplicate() => new SomeData_Goo(new SomeData(Value));
        public override string ToString() => Value.ToString();
    }

    public class SomeData_Param : GH_Param<SomeData_Goo>
    {
        public SomeData_Param(): base("SomeData", "SD", "Some data data data.", "SomeDataTests", "TestLib", GH_ParamAccess.item)
        {
        }

        public override Guid ComponentGuid => new Guid("{ f932581a-ab25-4b4e-9665-2a27f078673a } ");
    }
}