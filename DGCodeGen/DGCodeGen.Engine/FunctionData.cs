using System;
using System.Collections.Generic;
using DGCodeGen.Attributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DGCodeGen.Engine
{
    /// <summary>
    /// A collection of data about a function, which can then be written to Dynamo/Grasshopper
    /// </summary>
    public class FunctionData : ICodeItem
    {
        public string Namespace;
        public Type ParentClass;

        public DyFunc DynamoFunctionAttr;
        public GhFunc GrasshopperFunctionAttr;
        public DGDescription DescriptionAttr;



        public string MethodName;
        public string Name => MethodName;

        public List<InputData> Inputs;
        public List<OutputData> Outputs;

        public CodeDocument CodeDocument { get; set; }

        public MethodDeclarationSyntax MethodNode;
        public EvaluationStatements FunctionBody;

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
