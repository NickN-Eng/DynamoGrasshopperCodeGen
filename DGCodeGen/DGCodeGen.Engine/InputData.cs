using System;
using DGCodeGen.Attributes;

namespace DGCodeGen.Engine
{
    public class InputData
    {
        private Type _OriginalType;
        /// <summary>
        /// The type in the declaration method. This may need to be converted to a type appropriate for 
        /// </summary>
        public Type OriginalType 
        { 
            get => _OriginalType; 
            set 
            { 
                _OriginalType = value;
                IsList = ReflectionHelper.IsList(value, out Type iType);
                DGCommonType = IsList ? iType : value;
            } 
        }

        public Type DGCommonType { get; private set; }

        public bool IsList { get; private set; }

        /// <summary>
        /// The name of the parameter in c# code
        /// </summary>
        public string ParameterName;

        public DGInput InputAttr;

        public bool HasDefaultValue;

        public object DefaultValue;

        
    }


}
