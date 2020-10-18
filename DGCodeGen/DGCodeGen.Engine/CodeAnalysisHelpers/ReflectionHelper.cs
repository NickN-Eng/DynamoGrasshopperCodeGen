using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DGCodeGen.Engine
{
    public static class ReflectionHelper
    {
        private static readonly HashSet<Type> ValTupleTypes = new HashSet<Type>(new Type[]
        {
            typeof(ValueTuple<>), typeof(ValueTuple<,>),
            typeof(ValueTuple<,,>), typeof(ValueTuple<,,,>),
            typeof(ValueTuple<,,,,>), typeof(ValueTuple<,,,,,>),
            typeof(ValueTuple<,,,,,,>), typeof(ValueTuple<,,,,,,,>)
        });

        public static bool IsValueTuple(Type type)
        {
            return type.IsGenericType
                && ValTupleTypes.Contains(type.GetGenericTypeDefinition());
        }

        public static bool IsList(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
        }

        public static bool IsList(Type type, out Type itemType)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                itemType = type.GetGenericArguments()[0];
                return true;
            }
            itemType = null;
            return false;
        }
    }
}
