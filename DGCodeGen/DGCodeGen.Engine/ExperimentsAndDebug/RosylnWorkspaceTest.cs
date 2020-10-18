using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DGCodeGen.Engine
{
    // Requires more testing! This is the ideal way to load assemblies + compliation units, but seems a little unreliable on different machine setups.
    //public class RosylnWorkspaceTest
    //{

    //    public static void Test()
    //    {
    //        MSBuildLocator.RegisterDefaults();

    //        string rootPath = @"C:\Users\nniem\source\repos\DGCodeGen";
    //        //string rootPath = @"C:\Users\nicho\source\repos\DGCodeGen";

    //        string projectPath = rootPath + @"\TestLib\TestLib.Common2\TestLib.Common2.csproj";
    //        //string projectPath = rootPath + @"\TestLib\TestLib.Common\TestLib.Common.csproj";
    //        //string projectPath = rootPath + @"\TestLib\TestLib.Grasshopper\TestLib.Grasshopper.csproj";
    //        TestLoadProject(projectPath);

    //        Console.ReadKey();

    //        //string solutionPath = @"C:\Users\nicho\source\repos\RosylnTest\RosylnTest.sln";
    //        //solutionPath = @"C:\Users\nicho\source\repos\DynohopperGeneration\DynohopperGeneration.sln";
    //        string solutionPath = rootPath + @"\TestLib\TestLib.sln";
    //        TestLoadSolution(solutionPath);

    //        Console.ReadKey();
    //    }

    //    private static void TestLoadSolution(string solutionPath)
    //    {
    //        var msWorkspace = MSBuildWorkspace.Create();
    //        msWorkspace.WorkspaceFailed += MsWorkspace_WorkspaceFailed;
    //        var solution = msWorkspace.OpenSolutionAsync(solutionPath).Result;
    //        foreach (var project in solution.Projects)
    //        {
    //            foreach (var document in project.Documents)
    //            {
    //                Console.WriteLine(project.Name + ": " + document.Name);
    //            }
    //        }
    //    }


    //    private static void TestLoadProject(string projectPath)
    //    {
    //        var msWorkspace = MSBuildWorkspace.Create();
    //        msWorkspace.WorkspaceFailed += MsWorkspace_WorkspaceFailed;
    //        var project = msWorkspace.OpenProjectAsync(projectPath).Result;
    //        foreach (var document in project.Documents)
    //        {
    //            Console.WriteLine(project.Name + ": " + document.Name);
    //        }

    //    }
    //    private static void MsWorkspace_WorkspaceFailed(object sender, Microsoft.CodeAnalysis.WorkspaceDiagnosticEventArgs e)
    //    {
    //        Console.WriteLine(e.Diagnostic.Message);
    //        Console.ReadKey();
    //        throw new NotImplementedException();
    //    }
    //}
}
