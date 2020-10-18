using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


namespace DGCodeGen.Engine
{
    /// <summary>
    /// The App which should be called within the DGCommon library.
    /// </summary>
    public class App
    {
        public FileConfig FileConfig;
        public AssemblyAndProjData AssemblyAndProjData;
        
        public FunctionCollection Functions;
        public DataClassCollection DataClasses;

        public TypeDictionary TypeDictionary;

        public void Run()
        {
            Console.WriteLine("## Dynamo-Grasshopper Code Generation ##");
            Console.WriteLine();

            FileConfig = new FileConfig("TestLib.Grasshopper", "Dyn");
            AssemblyAndProjData = new AssemblyAndProjData(FileConfig);

            TypeDictionary = new TypeDictionary();
            TypeDictionary.LoadTypeOverrides(AssemblyAndProjData);

            var options = new List<(string optionText, Action action)>();

            //#DATACLASSES#
            //Load dataclasses
            Console.WriteLine("# Checking Dataclasses");
            DataClasses = DataClassCollection.Parse(AssemblyAndProjData);
            //Dataclass checking TBC (See DataClassChecker)
            var dataclassChecker = new DataClassChecker(this);
            bool DataclassCheckPassed = dataclassChecker.Parse(DataClasses);
            dataclassChecker.WriteToConsole();
            //Add dataclass writing options (if check passed)
            if(DataclassCheckPassed)
                options.Add(("Write/Update dataclasses", () => DataClassWriter_Grasshopper.QuickWriteAll(this, DataClasses)));
            else
                Console.WriteLine("Data class errors need to be fixed before writing them.");

            Console.WriteLine();

            //#FUNCTIONS#
            //Load functions
            Console.WriteLine("# Checking Functions");
            Functions = FunctionCollection.Parse(AssemblyAndProjData);

            //Function reporting TBC
            //Count functions declared, number of [Marked for update] etc... Number already written into grasshopper/dynamo libraries etc...

            //Check functions for errors
            var functionChecker = new FunctionChecker(this);
            bool FunctionCheckPassed = functionChecker.Parse(Functions);
            functionChecker.WriteToConsole();
            //Add function writing options (if check passed)
            if (FunctionCheckPassed)
                options.Add(  ("Write/Update functions", () => FunctionWriter_Grasshopper.QuickWriteAll(this, Functions)  )  );
            else
                Console.WriteLine("Function errors need to be fixed before writing functions.");

            Console.WriteLine();
            Console.WriteLine("# Actions!");

            if (options.Count == 0)
            {
                Console.WriteLine("No actions available. Please fix errors to Dataclasses/Functions.");
            }
            else if(options.Count == 1)
            {
                bool userResponse = ConsoleHelper.GetYesOrNo($"Would you like to {options[0].optionText}?");
                if (userResponse) options[0].action.Invoke();
            }
            else
            {
                Console.WriteLine("What would you like to do?");
                ConsoleHelper.DoActionByOptionNumber(options);
                Console.WriteLine("Finished!"); 
            }

            Console.WriteLine();
            Console.WriteLine("Hit any key to exit.");
            Console.ReadKey();
        }
    }
}
