using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DGCodeGen.Engine;

namespace DGCodeGen.TypeConversions
{
    /// <summary>
    /// This class is meant to be implemented in the DGCommon library, with strongly typed code.
    /// The Rosyln C# code reader can then convert it into a TypeConverter.
    /// </summary>
    public abstract class TypeConversionTemplate<DGComType, GhType, DyType>
    {
        public Type DGCommonType => typeof(DGComType);

        public Type GrasshopperType => typeof(GhType);

        public Type DynamoType => typeof(DyType);

        public abstract GhType Convert_DGCommonToGh(DGComType commonObj);
        public abstract DyType Convert_DGCommonToDy(DGComType commonObj);
        public abstract DGComType Convert_GhToDGCommon(GhType ghObj);
        public abstract DGComType Convert_DyToDGCommon(DyType dyObj);
    }
}
