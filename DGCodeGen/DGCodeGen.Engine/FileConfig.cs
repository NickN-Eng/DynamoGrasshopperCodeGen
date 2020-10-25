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
        /// <param name="dynamoProjName">The name of the Dynamo C# project. Set to null/"" if there is no dynamo project.</param>
        /// <param name="grasshopperProjName">The name of the Grasshopper C# project. Set to null/"" if there is no dynamo project.</param>
        /// <param name="grasshopperNamespace">The default namespace in which generated grasshopper code will reside.</param>
        /// <param name="dynamoNamespace">The default namespace in which generated dynamo code will reside.</param>
        /// <param name="toolName">The name of this tool. Currently only used as the tool ribbon name in Grasshopper.</param>
        public FileConfig(string grasshopperProjName, string dynamoProjName, string grasshopperNamespace, string dynamoNamespace, string toolName)
        {
            this.GrasshopperProjName = grasshopperProjName;
            this.DynamoProjName = dynamoProjName;

            DGCommonAssembly = Assembly.GetEntryAssembly();
            var path = new Uri(DGCommonAssembly.CodeBase).LocalPath;

            int dgCommonProjPathBinPosition = path.LastIndexOf("\\bin");
            DGCommonProjectFilepath = path.Substring(0, dgCommonProjPathBinPosition);

            int solutionRootPosition = DGCommonProjectFilepath.LastIndexOf('\\');
            SolutionFilepath = DGCommonProjectFilepath.Substring(0, solutionRootPosition);

            GrasshopperProjectFilepath = SolutionFilepath + "\\" + grasshopperProjName;
            DynamoProjectFilepath = SolutionFilepath + "\\" + dynamoProjName;

            //Requires that the DGCommon project references the grasshopper project - such that the Lib.Grasshopper.dll is copied to the DGCommon bin folder
            //Also requires that the grasshopper project does not delete the dll (remove delete dll from build events).
            //TBC - 
            //GrasshopperAssembly = Assembly.LoadWithPartialName(grasshopperProjName);

            //TODO - Automatically read from the default namespace in the csproj file
            GrasshopperNamespace = grasshopperNamespace;
            DynamoNamespace = dynamoNamespace;
            ToolName = toolName;
        }


        /*    All filepaths exclude the trailing '\'    */

        public bool HasGrasshopper => !string.IsNullOrEmpty(GrasshopperProjName);
        public bool HasDynamo => !string.IsNullOrEmpty(DynamoProjName);

        public string GrasshopperProjName { get; private set; }

        public string DynamoProjName { get; private set; }

        private string SolutionFilepath { get; set; }

        public Assembly DGCommonAssembly { get; private set; }

        //public Assembly GrasshopperAssembly { get; private set; }

        public string GrasshopperNamespace { get; private set; }
        public string DynamoNamespace { get; private set; }
        
        /// <summary>
        /// The name of this node library.
        /// This is used for the "Category" in a grasshopper plugin (i.e. the name in the Ribbon). 
        /// </summary>
        public string ToolName { get; private set; }
         

        /// <summary>
        /// The filepath to the DGCommon project
        /// </summary>
        public string DGCommonProjectFilepath { get; private set; }

        public string DynamoProjectFilepath { get; private set; }

        public string GrasshopperProjectFilepath { get; private set; }

    }
}
