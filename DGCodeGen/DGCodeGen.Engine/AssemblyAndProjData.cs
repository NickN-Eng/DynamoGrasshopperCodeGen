using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DGCodeGen.Engine
{
    /// <summary>
    /// A class which contains references to the loaded assembly (for reflection)
    /// and the parsed c# code.
    /// </summary>
    public class AssemblyAndProjData
    {
        public AssemblyAndProjData(FileConfig fileConfig)
        {
            DGCommonAssembly = Assembly.GetEntryAssembly();

            //Requires that the DGCommon project references the grasshopper project - such that the Lib.Grasshopper.dll is copied to the DGCommon bin folder
            //Also requires that the grasshopper project does not delete the dll (remove delete dll from build events).
            GrasshopperAssembly = Assembly.LoadWithPartialName(fileConfig.GrasshopperProjName);

            DGCommonCSharpDocs = new List<CodeDocument>();

            foreach (var filePath in Directory.GetFiles(fileConfig.DGCommonProjectFilepath, "*.cs"))
            {
                var doc = new CodeDocument() { FilePath = filePath };
                doc.Tree = CSharpSyntaxTree.ParseText(File.ReadAllText(filePath));
                doc.RootNode = doc.Tree.GetRoot();
                DGCommonCSharpDocs.Add(doc);
            }
        }

        public Assembly DGCommonAssembly { get; private set; }

        public Assembly GrasshopperAssembly { get; private set; }

        public List<CodeDocument> DGCommonCSharpDocs { get; private set; }

        /// <summary>
        /// Updates code documents where the CodeDocument.RequiresUpdate flag is set to true.
        /// </summary>
        public void UpdateCodeDocuments()
        {
            //Use a backup strategy for rewriting code files?? I.e. write as new file, and then delete/rename file when everything is complete!
            foreach (var doc in DGCommonCSharpDocs)
            {
                if (doc.RequiresUpdate)
                {
                    //Todo: Use text writer?
                    //Tries to overwrite the text file.
                    //If the text file cannot be edited, the updates will instead be saved to a file called .../Filename_DGEdit.cs
                    try
                    {
                        File.WriteAllText(doc.FilePath, doc.RootNode.ToFullString());
                    }
                    catch (Exception e)
                    {
                        var debug = e.Message;

                        var fileName = new FileInfo(doc.FilePath).Name;
                        Console.WriteLine($"Could not edit {fileName} due to {e.Message}. Writing updated file to {fileName.Replace(".cs", "_DGEdit.cs")}");

                        var newPath = doc.FilePath.Replace(".cs", "_DGEdit.cs");
                        File.WriteAllText(newPath, doc.RootNode.ToFullString());
                    }
                    doc.RequiresUpdate = false;
                }
            }
        }
    }

    /// <summary>
    /// A document read from the DGCommon library/Grasshopper library/Dynamo library
    /// Contains data leading back to the original script file for updating the script.
    /// </summary>
    public class CodeDocument
    {
        public string FilePath;

        public SyntaxTree Tree; //Potentially redundant
        public SyntaxNode RootNode;

        public bool RequiresUpdate = false;
    }
}
