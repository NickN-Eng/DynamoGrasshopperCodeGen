using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DGCodeGen.Engine;

namespace TestLib.DGCommon
{
    /// <summary>
    /// Runs DGCodeGen.App from the DGCommon library
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            var app = new App();
            app.Run();

        }
    }
}