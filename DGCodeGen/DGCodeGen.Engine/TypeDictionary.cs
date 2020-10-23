using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

using DGCodeGen.TypeConversions;

namespace DGCodeGen.Engine
{
    public class TypeDictionary
    {
        public Dictionary<Type, TypeConversion> TypeConverters;

        public TypeDictionary()
        {
            var typeConvertersToInitialise = new TypeConversion[]
            {
                new IntegerTypeConversion(),
                new DoubleTypeConversion(),
                new BooleanTypeConversion(),
                new StringTypeConversion(),
                new GrasshopperPoint3dConversion(),
                new GrasshopperVector3dConversion(),
                new GrasshopperLineConversion(),
                new GrasshopperPlaneConversion(),
                new GrasshopperBoxConversion(),
                new GrasshopperArcConversion()
            };

            TypeConverters = new Dictionary<Type, TypeConversion>();
            foreach (var typCon in typeConvertersToInitialise)
                TypeConverters[typCon.DGCommonType] = typCon;
        }

        public void LoadTypeOverrides(AssemblyAndProjData assemblyAndProjData)
        {
            var typeConversionTypes = assemblyAndProjData.DGCommonAssembly.GetTypes().Where(typ => typeof(TypeConversion).IsAssignableFrom(typ)).ToList();
            foreach (var typ in typeConversionTypes)
            {
                var typeCon = (TypeConversion)Activator.CreateInstance(typ);
                TypeConverters[typeCon.DGCommonType] = typeCon; //Override any type currently in the dictionary
            }
        }

        public bool TryGet(Type DGCommonType, out TypeConversion typeConversion)
        {
            return TypeConverters.TryGetValue(DGCommonType, out typeConversion);
        }

        public TypeConversion Get(Type DGCommonType)
        {
            //Do some error handling?? E.g. suggest to the user about adding Type converters??
            //TypeConverters.TryGetValue(DGCommonType, out TypeConversion result);
            //return result;
            return TypeConverters[DGCommonType];
        }

        public void AddDataClasses(DataClassCollection dataClassCollection)
        {
            foreach (var dataclass in dataClassCollection.DataClasses)
            {
                var dataclassType = dataclass.DGCommonDataclassType;
                TypeConverters[dataclassType] = new DataclassConversion(dataclassType);
            }
        }
    }
}
