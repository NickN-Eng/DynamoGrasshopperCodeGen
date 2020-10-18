using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DGCodeGen.Attributes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DGCodeGen.Engine
{
    public class DataClassCollection
    {
        public List<DataClass> DataClasses;

        public static DataClassCollection Parse(AssemblyAndProjData assemblyAndProjData)
        {
            var dataClasses = new List<DataClass>();

            ParseFunctionsByReflection(assemblyAndProjData.DGCommonAssembly, dataClasses);

            ParseFunctionsByCodeAnalysis(assemblyAndProjData.DGCommonCSharpDocs, dataClasses);

            return new DataClassCollection() { DataClasses = dataClasses };
        }

        private static void ParseFunctionsByReflection(Assembly assembly, List<DataClass> dataClasses)
        {
            //Parsing by reflection
            foreach (var typ in assembly.GetTypes())
            {
                var dyDataclass = (DyDataClass)Attribute.GetCustomAttribute(typ, typeof(DyDataClass));
                var ghDataclass = (GhDataClass)Attribute.GetCustomAttribute(typ, typeof(GhDataClass));

                if (dyDataclass == null && ghDataclass == null)
                    continue;

                var dataclass = new DataClass();
                dataclass.Namespace = typ.Namespace;
                dataclass.Classname = typ.Name;
                dataclass.GhDataClass = ghDataclass;
                dataclass.DyDataClass = dyDataclass;
                dataclass.DGDescription = (DGDescription)Attribute.GetCustomAttribute(typ, typeof(DGDescription));
                dataclass.ExistingGuidAttr = (GhGuid)Attribute.GetCustomAttribute(typ, typeof(GhGuid));
                dataClasses.Add(dataclass);
            }
        }

        /// <summary>
        /// Requires the functions list to be populated with method names
        /// </summary>
        private static void ParseFunctionsByCodeAnalysis(List<CodeDocument> docs, List<DataClass> dataClasses)
        {
            foreach (var doc in docs)
            {
                var classSyntaxes = doc.RootNode.DescendantNodes().Where(node => node is ClassDeclarationSyntax).Select(node => (ClassDeclarationSyntax)node).ToList();

                foreach (var classSyntax in classSyntaxes)
                {
                    //Check for namespaces too (need a node scanner which considers the namespace context
                    var dataclassToUpdate = dataClasses.FirstOrDefault(dc => dc.Classname == classSyntax.Identifier.Text);
                    if (dataclassToUpdate == null) continue;

                    dataclassToUpdate.ClassSyntax = classSyntax;
                    dataclassToUpdate.CodeDocument = doc;
                }
            }
        }
    }

    public class DataClass : ICodeItem
    {
        public string Classname;
        public string Name => Classname;
        public string Namespace;

        public DyDataClass DyDataClass;
        public GhDataClass GhDataClass;
        public DGDescription DGDescription;

        public CodeDocument CodeDocument { get; set; }

        public ClassDeclarationSyntax ClassSyntax;

        /// <summary>
        /// May be missing if this function does not have an existing Guid. In this case, a new guid will need to be created.
        /// </summary>
        public GhGuid ExistingGuidAttr;

        /// <summary>
        /// This variable is populated if there is a new Guid was created during this run of the App.
        /// This Guid will need to be recorded once the grasshopper functions have been written.
        /// </summary>
        public string NewGuid;



        public string GetGhGuid()
        {
            if (ExistingGuidAttr != null) return ExistingGuidAttr.Guid;
            if (NewGuid != null) return NewGuid;

            NewGuid = Guid.NewGuid().ToString();
            return NewGuid;
        }

    }
}
