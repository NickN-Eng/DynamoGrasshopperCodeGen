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

        public void Run(FileConfig fileConfig)
        {
            Console.WriteLine("## Dynamo-Grasshopper Code Generation ##");
            Console.WriteLine();

            FileConfig = fileConfig;
            
            AssemblyAndProjData = new AssemblyAndProjData(FileConfig);

            //Was having an error with loading RhinoCommon. Needed to manually copy RhinoCommon from nuget directory into bin folder.
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
            TypeDictionary.AddDataClasses(DataClasses);
            //Add dataclass writing options (if check passed)
            if (DataclassCheckPassed)
            {
                if(FileConfig.HasGrasshopper)
                    options.Add(("Write/Update grasshopper dataclasses", () => DataClassWriter_Grasshopper.QuickWriteAll(this, DataClasses)));
            }
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
            {
                if (FileConfig.HasGrasshopper)
                    options.Add(  ("Write/Update grasshopper functions", () => FunctionWriter_Grasshopper.QuickWriteAll(this, Functions)  )  );

                if (FileConfig.HasDynamo)
                    options.Add(("Write/Update dynamo functions", () => FunctionWriter_Dynamo.QuickWriteAll(this, Functions)));
            }
            else
                Console.WriteLine("Function errors need to be fixed before writing functions.");

            if (DataclassCheckPassed && FunctionCheckPassed)
            {
                options.Add(("Write/Update dataclasses & functions", 
                    () =>   { 
                                DataClassWriter_Grasshopper.QuickWriteAll(this, DataClasses); 
                                FunctionWriter_Grasshopper.QuickWriteAll(this, Functions); 
                            }));
            }

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
