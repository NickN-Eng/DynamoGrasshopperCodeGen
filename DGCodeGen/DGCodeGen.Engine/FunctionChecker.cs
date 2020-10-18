using DGCodeGen.TypeConversions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DGCodeGen.Engine
{
    public class FunctionChecker : CheckerBase<FunctionCollection, FunctionData>
    {
        /*
         Checks TBC:
            - Description missing??

         
         */

        //public Dictionary<FunctionData, List<string>> ErrorMessages;
        //public int ErrorCount;

        //public FunctionChecker(App app)
        //{
        //    App = app;
        //}

        //private App App;
        private FunctionData CurrentFunction;

        public FunctionChecker(App app) : base(app)
        {
        }

        protected override bool ParseImplementation(FunctionCollection functions)
        {
            ErrorMessages = new Dictionary<FunctionData, List<string>>();
            ErrorCount = 0;

            CheckFunctionNamesNotDuplicated(functions.Functions);

            foreach (var func in functions.Functions)
            {
                CurrentFunction = func;

                CheckValidExistingGuid(func);

                //Scan input variables
                foreach (var input in func.Inputs)
                {
                    CheckInputVariableTypeUsage(input, func);
                }

                //Scan function body
                CheckFunctionBodyReturnStatement(func);

                //Scan output variables
                foreach (var output in func.Outputs)
                {
                    CheckOutputVariableTypeUsage(output, func);
                }
            }

      

            return ErrorCount == 0;
        }

        private void CheckFunctionNamesNotDuplicated(List<FunctionData> functions)
        {
            //Dictionaries of the form <FunctionName(attribute),MethodName>
            Dictionary<string, string> GrasshopperFuncNames = new Dictionary<string, string>();
            Dictionary<string, string> DynamoFuncNames = new Dictionary<string, string>();

            foreach (var function in functions)
            {
                if(function.GrasshopperFunctionAttr != null)
                {
                    var ghFuncName = function.GrasshopperFunctionAttr.Name;
                    if (GrasshopperFuncNames.TryGetValue(ghFuncName, out string duplicateMethodName))
                    {
                        var errorMessage = $"The Grasshopper function name ({ghFuncName}) is already being used by the method {duplicateMethodName}. Please use another function name.";
                        AddError(errorMessage, function);
                    }
                    else
                    {
                        GrasshopperFuncNames[ghFuncName] = function.MethodName;
                    }
                }

                if (function.DynamoFunctionAttr != null)
                {
                    var dyFuncName = function.DynamoFunctionAttr.Name;
                    if (DynamoFuncNames.TryGetValue(dyFuncName, out string duplicateMethodName))
                    {
                        var errorMessage = $"The Grasshopper function name ({dyFuncName}) is already being used by the method {duplicateMethodName}. Please use another function name.";
                        AddError(errorMessage, function);
                    }
                    else
                    {
                        DynamoFuncNames[dyFuncName] = function.MethodName;
                    }
                }
            }
        }

        private void CheckValidExistingGuid(FunctionData func)
        {
            if (func.ExistingGuidAttr == null) return;

            if (Guid.TryParse(func.ExistingGuidAttr.Guid, out Guid result)) return;

            AddError($"The supplied GhGuid is not a valid Guid string. Correct the guid string, or remove this attribute entirely " +
                $"so that another Guid can be automatically created.");
        }

        private void CheckFunctionBodyReturnStatement(FunctionData func)
        {
            if (!func.FunctionBody.CanCreateStatementsAsVariable)
                AddError($"The function code needs to have only ONE return statement. Also in needs to be the LAST statement in the body block.");
        }

        private void CheckInputVariableTypeUsage(InputData input, FunctionData func)
        {
            if(!App.TypeDictionary.TryGet(input.DGCommonType, out TypeConversion typeConverter))
            {
                AddError($"The input variable {input.ParameterName}, with type {input.DGCommonType} cannot be found in the type dictionary. Please create a TypeConversion (see docs) or use an alternative type.");
                return;
            }

            //var typeConverter = App.TypeDictionary.Get(input.DGCommonType);
            if (func.GrasshopperFunctionAttr != null && typeConverter.GrasshopperType == null)
                AddError($"The input variable {input.ParameterName}, with type {input.DGCommonType} cannot be used in Grasshopper, " +
                    $"even though this function has been specified for Grasshopper through the GhFunc attribute. Please use an " +
                    $"alternative type or split this into two functions (one for Dynamo, and one for Grasshopper).");

            if (func.DynamoFunctionAttr != null && typeConverter.DynamoType == null)
                AddError($"The input variable {input.ParameterName}, with type {input.DGCommonType} cannot be used in Dynamo, " +
                    $"even though this function has been specified for Dynamo through the DyFunc attribute. Please use an " +
                    $"alternative type or split this into two functions (one for Dynamo, and one for Grasshopper).");
        }

        private void CheckOutputVariableTypeUsage(OutputData output, FunctionData func)
        {
            if (!App.TypeDictionary.TryGet(output.DGCommonType, out TypeConversion typeConverter))
            {
                AddError($"The input variable {output.ParameterName}, with type {output.DGCommonType} cannot be found in the type dictionary. Please create a TypeConversion (see docs) or use an alternative type.");
                return;
            }

            if (func.GrasshopperFunctionAttr != null && typeConverter.GrasshopperType == null)
                AddError($"The output variable {output.ParameterName}, with type {output.DGCommonType} cannot be used in Grasshopper, " +
                    $"even though this function has been specified for Grasshopper through the GhFunc attribute. Please use an " +
                    $"alternative type or split this into two functions (one for Dynamo, and one for Grasshopper).");

            if (func.DynamoFunctionAttr != null && typeConverter.DynamoType == null)
                AddError($"The output variable {output.ParameterName}, with type {output.DGCommonType} cannot be used in Dynamo, " +
                    $"even though this function has been specified for Dynamo through the DyFunc attribute. Please use an " +
                    $"alternative type or split this into two functions (one for Dynamo, and one for Grasshopper).");
        }

        /// <summary>
        /// Adds an error, using the CurrentFunction as the default function
        /// </summary>
        /// <param name="message"></param>
        private void AddError(string message) => AddError(message, CurrentFunction);

        //private void AddError(string message, FunctionData function)
        //{
        //    ErrorCount++;
        //    if (ErrorMessages.ContainsKey(function))
        //        ErrorMessages[function].Add(message);
        //    else
        //        ErrorMessages[function] = new List<string>() { message };
        //}

        //public void WriteToConsole()
        //{
        //    Console.WriteLine($"Functions have a total of {ErrorCount} errors.");
        //    if(ErrorMessages.Count > 0) Console.WriteLine();

        //    foreach (var kvp in ErrorMessages)
        //    {
        //        Console.WriteLine($"<{kvp.Key.MethodName}> at {kvp.Key.CodeDocument.FilePath}");
        //        foreach (var message in kvp.Value)
        //        {
        //            Console.WriteLine(message);
        //        }
        //        Console.WriteLine();
        //    }
        //}
    }
}
