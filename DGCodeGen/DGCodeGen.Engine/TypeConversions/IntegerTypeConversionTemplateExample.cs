using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper;
using Grasshopper.Kernel.Types;
using DGCodeGen.Engine;

namespace DGCodeGen.TypeConversions
{
    public class IntegerTypeConversionTemplateExample : TypeConversionTemplate<int, GH_Integer, int>
    {
        public override int Convert_DGCommonToDy(int commonObj) => commonObj;

        public override GH_Integer Convert_DGCommonToGh(int commonObj) => new GH_Integer(commonObj);

        public override int Convert_DyToDGCommon(int dyObj) => dyObj;

        public override int Convert_GhToDGCommon(GH_Integer ghObj) => ghObj.Value;
    }


}
