using DGCodeGen.Attributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DGCodeGen.Engine
{
    public class FunctionCollection
    {
        public List<FunctionData> Functions;

        public static FunctionCollection Parse(AssemblyAndProjData assemblyAndProjData)
        {
            var functions = new List<FunctionData>();

            ParseFunctionsByReflection(assemblyAndProjData.DGCommonAssembly, functions);

            ParseFunctionsByCodeAnalysis(assemblyAndProjData.DGCommonCSharpDocs, functions);

            return new FunctionCollection() { Functions = functions };
        }

        private static void ParseFunctionsByReflection(Assembly assembly, List<FunctionData> functionsToAppend)
        {
            //Parsing by reflection
            foreach (var typ in assembly.GetTypes())
            {
                foreach (var meth in typ.GetMethods())
                {
                    var dgFunc = (DGFunc)Attribute.GetCustomAttribute(meth, typeof(DGFunc));
                    var dyFunc = (DyFunc)Attribute.GetCustomAttribute(meth, typeof(DyFunc));
                    var ghFunc = (GhFunc)Attribute.GetCustomAttribute(meth, typeof(GhFunc));

                    if (dgFunc == null && dyFunc == null && ghFunc == null)
                        continue;

                    var func = new FunctionData();
                    functionsToAppend.Add(func);

                    func.MethodName = meth.Name;
                    func.DynamoFunctionAttr = dyFunc == null ? dgFunc?.ToDyFunc() : dyFunc;
                    func.GrasshopperFunctionAttr = ghFunc == null ? dgFunc?.ToGhFunc() : ghFunc;
                    func.DescriptionAttr = (DGDescription)Attribute.GetCustomAttribute(meth, typeof(DGDescription));
                    func.ExistingGuidAttr = (GhGuid)Attribute.GetCustomAttribute(meth, typeof(GhGuid));
                    func.Namespace = typ.Namespace;
                    func.ParentClass = typ; 

                    //Parse input parameters
                    var inputs = new List<InputData>();
                    func.Inputs = inputs;
                    foreach (var param in meth.GetParameters())
                    {
                        var input = new InputData();
                        input.InputAttr = (DGInput)Attribute.GetCustomAttribute(param, typeof(DGInput));
                        input.OriginalType = param.ParameterType;
                        input.ParameterName = param.Name;
                        input.HasDefaultValue = param.HasDefaultValue;
                        input.DefaultValue = param.DefaultValue;

                        inputs.Add(input);
                    }

                    //Parse output parameters
                    var outputs = new List<OutputData>();
                    func.Outputs = outputs;
                    var returnParm = meth.ReturnParameter;
                    if (ReflectionHelper.IsValueTuple(returnParm.ParameterType))
                    {
                        foreach (var tupleParam in returnParm.ParameterType.GetFields())
                        {
                            var output = new OutputData();
                            output.OriginalType = tupleParam.FieldType;
                            output.ParameterName = tupleParam.Name;
                            outputs.Add(output);
                        }
                    }
                    else
                    {
                        var output = new OutputData();
                        output.OriginalType = returnParm.ParameterType;
                        output.ParameterName = "result";
                        outputs.Add(output);
                    }

                    //Match output attributes to output parameters
                    var methodAttributes = Attribute.GetCustomAttributes(meth).Where(att => att is DGOutput).ToList();
                    if (methodAttributes.Count != outputs.Count)
                        throw new Exception($"Number of DGOutput attributes ({methodAttributes.Count}) should match the number of return parameters ({outputs.Count}), for the method: {meth.Name}.");

                    for (int i = 0; i < outputs.Count; i++)
                        outputs[i].OutputAttr = (DGOutput)methodAttributes[i];
                }
            }
        }

        /// <summary>
        /// Requires the functions list to be populated with method names
        /// </summary>
        private static void ParseFunctionsByCodeAnalysis(List<CodeDocument> docs, List<FunctionData> functionsToAppend)
        {
            foreach (var doc in docs)
            {
                var methods = doc.RootNode.DescendantNodes().Where(node => node is MethodDeclarationSyntax).Select(node => (MethodDeclarationSyntax)node).ToList();

                foreach (var meth in methods)
                {
                    //TBC - Check for namespaces too
                    var function = functionsToAppend.FirstOrDefault(func => meth.Identifier.Text == func.MethodName);
                    if (function == null) continue;

                    function.CodeDocument = doc;
                    
                    PopulateFunction(meth, function);
                }

            }   
        }

        private static void PopulateFunction(MethodDeclarationSyntax methodNode, FunctionData dGFunction)
        {
            dGFunction.MethodNode = methodNode;
            dGFunction.FunctionBody = EvaluationStatements.FromMethod(methodNode);

            //This issue is flagged up in the FunctionChecker
            //if(!dGFunction.FunctionBody.CanCreateStatementsAsVariable)
            //    throw new Exception("Functions must be written so that there is only 1 return statement, at the END of the function.");
        }
    }
}
