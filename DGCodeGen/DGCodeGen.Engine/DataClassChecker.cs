using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DGCodeGen.Engine
{
    public class DataClassChecker : CheckerBase<DataClassCollection, DataClass>
    {
        /*
          Checks TBC:
            - missing description...
            - Has a self duplicating constructor
            - Existing Guid is valid (if any)

          Warnings TBC
            - Has a ToString implementation
         
         */

        public DataClassChecker(App app) : base(app)
        {
        }

        protected override bool ParseImplementation(DataClassCollection collection)
        {
            return true;
        }
    }
}
