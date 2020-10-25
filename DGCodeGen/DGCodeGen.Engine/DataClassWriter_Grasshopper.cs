using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static DGCodeGen.Engine.SyntaxHelper;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;

namespace DGCodeGen.Engine
{
    public class DataClassWriter_Grasshopper
    {
        private App App;

        public List<UsingDirectiveSyntax> Usings;

        public DataClassWriter_Grasshopper(App app)
        {
            App = app;

            //Create default usings for test
            Usings = new List<UsingDirectiveSyntax>
            {
                UsingDirective( IdentifierName("System")),
                UsingDirective(
                    QualifiedName(
                        QualifiedName(
                            IdentifierName("System"),
                            IdentifierName("Collections")),
                        IdentifierName("Generic"))),
                UsingDirective(
                    QualifiedName(
                        IdentifierName("Grasshopper"),
                        IdentifierName("Kernel"))),
                UsingDirective(AutoChainedName("Grasshopper","Kernel","Types")),
                UsingDirective(
                    QualifiedName(
                        IdentifierName("Rhino"),
                        IdentifierName("Geometry")))
            };
        }

        public static void QuickWriteAll(App app, DataClassCollection dataclassCollection)
        {
            var functionWriter = new DataClassWriter_Grasshopper(app);
            functionWriter.WriteAll(dataclassCollection);
        }

        public void WriteAll(DataClassCollection dataclasses)
        {
            if (dataclasses.DataClasses.Count == 0) return;

            var generatedNodeFilepath = App.FileConfig.GrasshopperProjectFilepath + @"\DGCodeGen\";
            Directory.CreateDirectory(generatedNodeFilepath);

            foreach (var dc in dataclasses.DataClasses)
            {
                var text = Generate(dc).ToFullString();
                //var text = ghopperFunc.Generate().ToFullString();
                //Console.WriteLine(text);

                File.WriteAllText($"{generatedNodeFilepath}{dc.Classname}.cs", text);
            }

            //Update the attribute for the guid
            foreach (var dc in dataclasses.DataClasses)
            {
                if (dc.NewGuid == null) return;
                UpdateAttrInCodeDocuments(dc);
            }

            //Overwrite CodeDocuments
            App.AssemblyAndProjData.UpdateCodeDocuments();
        }

        private static void UpdateAttrInCodeDocuments(DataClass dataclass)
        {
            var existingAttributeList = dataclass.ClassSyntax.AttributeLists;
            var guidAttribute = AttributeWithArgs("GhGuid", AttrStringLitArg(dataclass.NewGuid))
                                    .WithTriviaFrom(existingAttributeList.Last()); //Assumes the last attribute is not the first

            var newAttributes = existingAttributeList.Add(guidAttribute);
            var newClassNode = dataclass.ClassSyntax.WithAttributeLists(newAttributes);

            //A bit of a hack to find the correct node to replace
            //This is required because the SyntaxNode object references change each time the tree is transformed
            //Therefore the ClassNode references within each dataclass are no longer valid once the tree is edited.
            //So we are search for by Class name, in order to find the node to replace
            var className = dataclass.ClassSyntax.Identifier.Text;
            var classNodeInNewTree = dataclass.CodeDocument.RootNode.DescendantNodes()
                .Where(node => node is ClassDeclarationSyntax classSyn && classSyn.Identifier.Text == className).First();

            dataclass.CodeDocument.RootNode = dataclass.CodeDocument.RootNode.ReplaceNode(classNodeInNewTree, newClassNode);
            dataclass.CodeDocument.RequiresUpdate = true;
        }

        private CompilationUnitSyntax Generate(DataClass dc)
        {
            string className = dc.Classname.FirstCharToUpper(); //ensure that className is Capitalised, so that classNameCamel =/= className
            string classNameCamel = dc.Classname.FirstCharToLower();
            string guid = dc.GetGhGuid();
            string nickName = dc.GhDataClass.NickName;
            string description = dc.DGDescription.Description;
            string category = dc.GhDataClass.Category;
            string subcategory = dc.GhDataClass.Subcategory;
            string gooCode = $@"
            public class {className}_Goo : GH_Goo<{className}>
            {{
                public override bool IsValid => true;

                public override string TypeName => ""{className}"";

                public override string TypeDescription => ""A bit of data."";

                public {className}_Goo({className} {classNameCamel})
                {{
                    Value = {classNameCamel};
                }}

                public override IGH_Goo Duplicate() => new {className}_Goo(new {className}(Value));

                public override string ToString() => Value.ToString();
            }}";

            string paramCode = $@"
            public class {className}_Param : GH_Param<{className}_Goo>
            {{
                public {className}_Param() : base(""{className}"", ""{nickName}"", ""{description}"", ""{category}"", ""{subcategory}"", GH_ParamAccess.item) {{ }}

                public override Guid ComponentGuid => new Guid(""{{ {guid} }} "");
            }}";

            var dataclass = dc.ClassSyntax.WithAttributeLists(AttrList()); //Remove the attributes on the dc class
            var gooClass = QuickParse<ClassDeclarationSyntax>(gooCode);
            var paramClass = QuickParse<ClassDeclarationSyntax>(paramCode);

            var classList = NodeList(dataclass, gooClass, paramClass);

            CompilationUnitSyntax result = CompilationUnit()
                    .WithUsings(List(Usings))
                    //.WithMembers( SingletonList<MemberDeclarationSyntax>( NamespaceDeclaration( QualifiedName( IdentifierName("JointSolver2D"), IdentifierName("Grasshopper")))
                    .WithMembers(SingletonList<MemberDeclarationSyntax>(GetNamespaceFromString(App.FileConfig.GrasshopperNamespace)
                        .WithMembers(classList)));

            result = result.NormalizeWhitespace();
            return result;

            return null;
        }

    }
}
