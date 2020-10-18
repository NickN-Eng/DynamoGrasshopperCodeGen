using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DGCodeGen.Engine
{
    /// <summary>
    /// A class which contains file settings for the generator.
    /// </summary>
    public class FileConfig
    {
        /// <summary>
        /// Initialises the file config
        /// ASSUMING this is constructor called from the DGCommon assembly (e.g. as a console app)
        /// Reason being, this function uses Assembly.GetExecutingAssembly to populate the reference/filepath to CommonAssembly and filepath.
        /// </summary>
        /// <param name="DynamoProjName"></param>
        /// <param name="GrasshopperProjName"></param>
        public FileConfig(string GrasshopperProjName, string DynamoProjName)
        {
            this.GrasshopperProjName = GrasshopperProjName;
            this.DynamoProjName = DynamoProjName;

            DGCommonAssembly = Assembly.GetEntryAssembly();
            var path = new Uri(DGCommonAssembly.CodeBase).LocalPath;

            int dgCommonProjPathBinPosition = path.LastIndexOf("\\bin");
            DGCommonProjectFilepath = path.Substring(0, dgCommonProjPathBinPosition);

            int solutionRootPosition = DGCommonProjectFilepath.LastIndexOf('\\');
            SolutionFilepath = DGCommonProjectFilepath.Substring(0, solutionRootPosition);

            GrasshopperProjectFilepath = SolutionFilepath + "\\" + GrasshopperProjName;

            //Requires that the DGCommon project references the grasshopper project - such that the Lib.Grasshopper.dll is copied to the DGCommon bin folder
            //Also requires that the grasshopper project does not delete the dll (remove delete dll from build events).
            GrasshopperAssembly = Assembly.LoadWithPartialName(GrasshopperProjName);

            GrasshopperNamespace = GrasshopperAssembly.GetTypes().First().Namespace;
        }


        /*    All filepaths exclude the trailing '\'    */

        public string SolutionFilepath { get; private set; }

        public Assembly DGCommonAssembly { get; private set; }

        public Assembly GrasshopperAssembly { get; private set; }

        public string GrasshopperNamespace { get; private set; }

        /// <summary>
        /// The filepath to the DGCommon project
        /// </summary>
        public string DGCommonProjectFilepath { get; private set; }

        public string DynamoProjectFilepath { get; private set; }

        public string GrasshopperProjectFilepath { get; private set; }

        public string GrasshopperProjName { get; private set; }

        public string DynamoProjName { get; private set; }
    }
}
