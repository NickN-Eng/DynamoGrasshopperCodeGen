using DGCodeGen.Attributes;
using System;

namespace DGCodeGen.Engine
{
    public class OutputData
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

        public string ParameterName;

        public DGOutput OutputAttr;
    }


}
