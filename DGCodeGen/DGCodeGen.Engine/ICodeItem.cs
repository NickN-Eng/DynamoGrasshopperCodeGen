using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DGCodeGen.Engine
{
    /// <summary>
    /// An interface for object representations of code e.g. Function/DataClass
    /// </summary>
    public interface ICodeItem
    {
        CodeDocument CodeDocument { get; }
        string Name { get; }
    }
}
