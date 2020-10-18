using Microsoft.CodeAnalysis.CSharp;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static DGCodeGen.Engine.SyntaxHelper;
using System.IO;

namespace DGCodeGen.Engine
{
    public class FunctionWriter_Grasshopper
    {
        private App App;

        public List<UsingDirectiveSyntax> Usings;

        //A temp variable which is reset for every function generation
        private List<MemberDeclarationSyntax> ClassElements;

        public FunctionWriter_Grasshopper(App app)
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
                UsingDirective(
                    QualifiedName(
                        IdentifierName("Rhino"),
                        IdentifierName("Geometry")))
            };
        }

        public static void QuickWriteAll(App app, FunctionCollection functionCollection)
        {
            var functionWriter = new FunctionWriter_Grasshopper(app);
            functionWriter.WriteAll(functionCollection);
        }

        public void WriteAll(FunctionCollection functions)
        {
            if (functions.Functions.Count == 0) return;

            var generatedNodeFilepath = App.FileConfig.GrasshopperProjectFilepath + @"\DGCodeGen\";
            Directory.CreateDirectory(generatedNodeFilepath);

            foreach (var func in functions.Functions)
            {
                var text = Generate(func).ToFullString();
                //var text = ghopperFunc.Generate().ToFullString();
                //Console.WriteLine(text);

                File.WriteAllText($"{generatedNodeFilepath}{func.GrasshopperFunctionAttr.Name}_Component.cs", text);
            }

            //Update the attribute for the guid
            foreach (var func in functions.Functions)
            {
                if (func.NewGuid == null) return;
                UpdateAttrInCodeDocuments(func);
            }

            //Overwrite CodeDocuments
            App.AssemblyAndProjData.UpdateCodeDocuments();
        }

        private static void UpdateAttrInCodeDocuments(FunctionData func)
        {
            var existingAttributeList = func.MethodNode.AttributeLists;
            var guidAttribute = AttributeWithArgs("GhGuid", AttrStringLitArg(func.NewGuid))
                                    .WithTriviaFrom(existingAttributeList.Last()); //Assumes the last attribute is not the first

            var newAttributes = existingAttributeList.Add(guidAttribute);
            var newMethNode = func.MethodNode.WithAttributeLists(newAttributes);

            //A bit of a hack to find the correct node to replace
            //This is required because the SyntaxNode object references change each time the tree is transformed
            //Therefore the MethodNode references within each function are no longer valid once the tree is edited.
            //So we are search for by method name, in order to find the node to replace
            var methName = func.MethodNode.Identifier.Text;
            var methodNodeInNewTree = func.CodeDocument.RootNode.DescendantNodes()
                .Where(node => node is MethodDeclarationSyntax meth && meth.Identifier.Text == methName).First();

            func.CodeDocument.RootNode = func.CodeDocument.RootNode.ReplaceNode(methodNodeInNewTree, newMethNode);
            func.CodeDocument.RequiresUpdate = true;
        }

        public CompilationUnitSyntax Generate(FunctionData functionData)
        {
            var ComponentName = functionData.GrasshopperFunctionAttr.Name;

            ClassElements = new List<MemberDeclarationSyntax>();
            
            var constructor = ComponentConstructor(
                functionData.GrasshopperFunctionAttr.Name,
                functionData.GrasshopperFunctionAttr.NickName,
                functionData.DescriptionAttr.Description,
                functionData.GrasshopperFunctionAttr.Category,
                functionData.GrasshopperFunctionAttr.Subcategory
                );
            ClassElements.Add(constructor);
            ClassElements.Add(ExposureProperty());
            ClassElements.Add(IconProperty());
            ClassElements.Add(GuidProperty(functionData.GetGhGuid()));

            ClassElements.Add(RegisterInputsMethod(functionData.Inputs));
            ClassElements.Add(RegisterOutputsMethod(functionData.Outputs));
            ClassElements.Add(SolveInstanceMethod(functionData));

            CompilationUnitSyntax result = CompilationUnit()
                    .WithUsings(List(Usings))
                    //.WithMembers( SingletonList<MemberDeclarationSyntax>( NamespaceDeclaration( QualifiedName( IdentifierName("JointSolver2D"), IdentifierName("Grasshopper")))
                    .WithMembers(SingletonList<MemberDeclarationSyntax>(GetNamespaceFromString(App.FileConfig.GrasshopperNamespace)
                        .WithMembers(
                            SingletonList<MemberDeclarationSyntax>(
                                ClassDeclaration($"{ComponentName}_Component") //Class name
                                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword))) //public class
                                .WithBaseList(BaseList(SingletonSeparatedList<BaseTypeSyntax>(SimpleBaseType(IdentifierName("GH_Component"))))) //Inherit type
                                .WithMembers(List(ClassElements))))));

            result = result.NormalizeWhitespace();
            return result;
        }

        public ConstructorDeclarationSyntax ComponentConstructor(string name, string nickname, string description, string category, string subcategory)
        {
            return ConstructorDeclaration(Identifier("CreateNode_Component")).WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                        .WithInitializer(ConstructorInitializer(SyntaxKind.BaseConstructorInitializer, ArgumentList(SeparatedList<ArgumentSyntax>(new SyntaxNodeOrToken[]
                        {
                            Argument(LiteralExpression(SyntaxKind.StringLiteralExpression,Literal(name))),
                            Token(SyntaxKind.CommaToken),
                            Argument(LiteralExpression(SyntaxKind.StringLiteralExpression,Literal(nickname))),
                            Token(SyntaxKind.CommaToken),
                            Argument(LiteralExpression(SyntaxKind.StringLiteralExpression,Literal(description))),
                            Token(SyntaxKind.CommaToken),
                            Argument(LiteralExpression(SyntaxKind.StringLiteralExpression,Literal(category))),
                            Token(SyntaxKind.CommaToken),
                            Argument(LiteralExpression(SyntaxKind.StringLiteralExpression,Literal(subcategory)))
                        }))))
                        .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
        }

        private MemberDeclarationSyntax ExposureProperty()
        {
            /*
                    public override GH_Exposure Exposure => GH_Exposure.primary; 
             */

            return PropertyDeclaration(IdentifierName("GH_Exposure"), Identifier("Exposure")) //Return type AND property name
                            .WithModifiers(TokenList(new[] { Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.OverrideKeyword) })) //Access modifiers e.g. public, override
                            .WithExpressionBody(ArrowExpressionClause(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,IdentifierName("GH_Exposure"),IdentifierName("primary")))) // Arrow, Expression contents
                            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)); //Finishing colon
        }

        private MemberDeclarationSyntax IconProperty()
        {
            /*
                    protected override System.Drawing.Bitmap Icon => null;
             */

            return PropertyDeclaration(AutoChainedName("System","Drawing","Bitmap"),Identifier("Icon")) //Return type AND property name
                            .WithModifiers(TokenList(new[]{Token(SyntaxKind.ProtectedKeyword),Token(SyntaxKind.OverrideKeyword)})) //Access modifiers e.g. protected, override
                            .WithExpressionBody(ArrowExpressionClause(LiteralExpression(SyntaxKind.NullLiteralExpression))) // Arrow, Expression contents
                            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)); //Finishing colon
        }

        private MemberDeclarationSyntax GuidProperty(string guid)
        {
            /*
                    public override Guid ComponentGuid => new Guid("1083d3ba-c6e4-43de-abbc-dbdf04db36a0");
             */

            return PropertyDeclaration(IdentifierName("Guid"), Identifier("ComponentGuid")) //Return type AND property name
                            .WithModifiers(TokenList(new[] { Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.OverrideKeyword) })) //Access modifiers e.g. public, override
                            .WithExpressionBody(ArrowExpressionClause(ObjectCreationExpression(IdentifierName("Guid"))
                                .WithArgumentList(ArgumentList(SingletonSeparatedList(Argument(LiteralExpression(SyntaxKind.StringLiteralExpression,Literal(guid)))))))) // Arrow, Expression contents
                            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)); //Finishing colon
        }

        private MethodDeclarationSyntax RegisterInputsMethod(List<InputData> inputs)
        {
            /*
                protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
                {
                    pManager.AddIntegerParameter("Int", "I", "Description", GH_ParamAccess.item);
                }
             */
            var statements = new List<StatementSyntax>();
            foreach (var inp in inputs)
            {
                var typConverter = App.TypeDictionary.Get(inp.DGCommonType);
                var accessType = MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName("GH_ParamAccess"), IdentifierName(inp.IsList ? "list" : "item"));

                ArgumentListSyntax argsList;
                if (typConverter.Gh_CustomParameterName == null)
                    argsList = ArgList(StringLitArg(inp.InputAttr.Name), StringLitArg(inp.InputAttr.NickNameGh), StringLitArg(inp.InputAttr.Description), Argument(accessType));
                else
                {
                    var newParamExpr = ObjectCreationExpression(IdentifierName(typConverter.Gh_CustomParameterName)).WithArgumentList(ArgumentList());
                    argsList = ArgList(Argument(newParamExpr), StringLitArg(inp.InputAttr.Name), StringLitArg(inp.InputAttr.NickNameGh), StringLitArg(inp.InputAttr.Description), Argument(accessType));
                }

                var expression = InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName("pManager"), IdentifierName(typConverter.Gh_AddParameterMethodName)))
                                    .WithArgumentList(argsList);
                statements.Add(ExpressionStatement(expression));
            }

            return MethodDeclaration(PredefinedType(Token(SyntaxKind.VoidKeyword)), Identifier("RegisterInputParams"))
                .WithModifiers(TokenList(new[] { Token(SyntaxKind.ProtectedKeyword), Token(SyntaxKind.OverrideKeyword) }))
                .WithParameterList(ParameterList(SingletonSeparatedList(Parameter(Identifier("pManager")).WithType(AutoChainedName("GH_Component", "GH_InputParamManager")))))
                .WithBody(Block(List(statements)));
        }



        private MemberDeclarationSyntax RegisterOutputsMethod(List<OutputData> outputs)
        {
            /*
                protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
                {
                    pManager.AddParameter(new SomeData_Param(), "Some data", "SDx", "Some description", GH_ParamAccess.item);
                }
             */
            var statements = new List<StatementSyntax>();
            foreach (var outp in outputs)
            {
                var typConverter = App.TypeDictionary.Get(outp.DGCommonType);
                var accessType = MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName("GH_ParamAccess"), IdentifierName(outp.IsList ? "list" : "item"));

                ArgumentListSyntax argsList;
                if (typConverter.Gh_CustomParameterName == null)
                    argsList = ArgList(StringLitArg(outp.OutputAttr.Name), StringLitArg(outp.OutputAttr.NickNameGh), StringLitArg(outp.OutputAttr.Description), Argument(accessType));
                else
                {
                    var newParamExpr = ObjectCreationExpression(IdentifierName(typConverter.Gh_CustomParameterName)).WithArgumentList(ArgumentList());
                    argsList = ArgList(Argument(newParamExpr), StringLitArg(outp.OutputAttr.Name), StringLitArg(outp.OutputAttr.NickNameGh), StringLitArg(outp.OutputAttr.Description), Argument(accessType));
                }

                var expression = InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName("pManager"), IdentifierName(typConverter.Gh_AddParameterMethodName)))
                                    .WithArgumentList(argsList);
                statements.Add(ExpressionStatement(expression));
            }

            return MethodDeclaration(PredefinedType(Token(SyntaxKind.VoidKeyword)), Identifier("RegisterOutputParams"))
                .WithModifiers(TokenList(new[] { Token(SyntaxKind.ProtectedKeyword), Token(SyntaxKind.OverrideKeyword) }))
                .WithParameterList(ParameterList(SingletonSeparatedList(Parameter(Identifier("pManager")).WithType(AutoChainedName("GH_Component", "GH_OutputParamManager")))))
                .WithBody(Block(List(statements)));
        }

        private MemberDeclarationSyntax SolveInstanceMethod(FunctionData functionData)
        {
            var statements = new List<StatementSyntax>();

            for (int i = 0; i < functionData.Inputs.Count; i++)
            {
                SolveInstance_CreateInputStatements(functionData.Inputs[i], i, statements);
            }

            //Put in function eval statements
            var functionEvalStatements = functionData.FunctionBody;
            if (!functionEvalStatements.CanCreateStatementsAsVariable)
                throw new Exception("Functions must be written so that there is only 1 return statement, at the END of the function.");

            List<StatementSyntax> funcStatementList;
            if (functionEvalStatements.IsTupleReturn)
            {
                var tupleNames = functionData.Outputs.Select(output => output.ParameterName).ToArray();
                funcStatementList = functionEvalStatements.CreateStatementsAsTupleVariables(tupleNames);
            }
            else
            {
                var outputParamName = functionData.Outputs[0].ParameterName;
                funcStatementList =  functionEvalStatements.CreateStatementsAsVariable(outputParamName);
            }
            funcStatementList.AddCommentToBeginning("//Function body");
            statements.AddRange(funcStatementList);

            for (int i = 0; i < functionData.Outputs.Count; i++)
            {
                SolveInstance_CreateOutputStatements(functionData.Outputs[i], i, statements);
            }

            return MethodDeclaration(PredefType(SyntaxKind.VoidKeyword), Identifier("SolveInstance"))
                .WithModifiers(TokenList(new[]{Token(SyntaxKind.ProtectedKeyword),Token(SyntaxKind.OverrideKeyword)}))
                .WithParameterList(ParamList(("DA", IdentifierName("IGH_DataAccess"))))
                .WithBody(Block(List(statements)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputData"></param>
        /// <param name="index"></param>
        /// <param name="statementsToAppend"></param>
        /// <returns></returns>
        private void SolveInstance_CreateInputStatements(InputData inputData, int index, List<StatementSyntax> statementsToAppend)
        {
            //TODO - move all conversion code into a static class
            //Then they will all be single line expressions... can use variableName_afterConv = variableName_prevConv.Select(item => Converter.VarTypeToVarType(item)).ToList();

            var typeCon = App.TypeDictionary.Get(inputData.DGCommonType);
            if (typeCon.GrasshopperType == null)
                throw new Exception("This type converter does not provide a grasshopper type. Please use a different type for this input variable.");

            var variableName_afterConv = inputData.ParameterName;
            var variableName_preConv = $"{inputData.ParameterName}_gh";
            SingleLineEvaluationStatements conversionStatements = typeCon.ConversionCode_GhToDGCommon(variableName_preConv);
            bool conversionRequired = conversionStatements != null;

            var variableName_InDaGet = conversionRequired ? variableName_preConv : variableName_afterConv;
            var variableType_gh = IdentifierName(typeCon.GrasshopperType.Name);
            var variableType = IdentifierName(inputData.DGCommonType.Name);
            if (inputData.IsList)
            {
                var newListStatement = NewListVariableDeclaration(variableType_gh, variableName_InDaGet);
                newListStatement = newListStatement.WithLeadingTrivia(CommentTriv($"//Fetching node input: {variableName_afterConv}."));
                statementsToAppend.Add(newListStatement);

                var daGetExpression = CallMethodOnObject("DA", "GetDataList", NumberArg(index), NamedArg(variableName_InDaGet));
                var ifStatement = IfStatement(daGetExpression.NotOnExpr(), ReturnStatement());
                statementsToAppend.Add(ifStatement);

                if (conversionRequired)
                {
                    //List conversion expression: (also add using System.Linq;)
                    //var variableName_afterConv = variableName_prevConv.Select(item => Converter.VarTypeToVarType(item)).ToList();
                    
                    var itemConversionExpression = typeCon.ConversionCode_GhToDGCommon("item").ExpressionNode;
                    var listConversionExpression = CallMethodOnObject(variableName_preConv, "Select", Argument(LambdaExpr("item", itemConversionExpression))).CallMethodOnExpr("ToList");
                    var listConversionStatement = listConversionExpression.AssignToNewVariable(ListType(variableType), variableName_afterConv).ToStatement();

                    statementsToAppend.Add(listConversionStatement);
                }
            }
            else
            {
                InsertEvaluationStatements(typeCon.Gh_GetInitialisationDefault(), variableName_InDaGet, statementsToAppend, $"//Fetching node input: {variableName_afterConv}.");

                var daGetExpression = CallMethodOnObject("DA", "GetData", NumberArg(index), NamedArgWithRef(variableName_InDaGet));
                var ifStatement = IfStatement(daGetExpression.NotOnExpr(), ReturnStatement());
                statementsToAppend.Add(ifStatement);

                if (conversionRequired)
                    InsertEvaluationStatements(conversionStatements, variableName_afterConv, statementsToAppend, null, (variableName_preConv, variableType_gh));
            }
        }

        private void SolveInstance_CreateOutputStatements(OutputData outputData, int index, List<StatementSyntax> statementsToAppend)
        {
            //TODO - move all conversion code into a static class
            //Then they will all be single line expressions... can use variableName_afterConv = variableName_prevConv.Select(item => Converter.VarTypeToVarType(item)).ToList();

            var typeCon = App.TypeDictionary.Get(outputData.DGCommonType);
            if (typeCon.GrasshopperType == null)
                throw new Exception("This type converter does not provide a grasshopper type. Please use a different type for this output variable.");

            var variableName_preConv = outputData.ParameterName;
            var variableName_afterConv = $"{outputData.ParameterName}_gh";
            SingleLineEvaluationStatements conversionStatements = typeCon.ConversionCode_DGCommonToGh(variableName_preConv);
            bool conversionRequired = conversionStatements != null;

            var variableName_InDaSet = conversionRequired ? variableName_afterConv : variableName_preConv;
            var variableType = IdentifierName(outputData.DGCommonType.Name);
            var variableType_gh = IdentifierName(typeCon.GrasshopperType.Name);

            var comment = $"//Setting node output: {variableName_preConv}.";

            if (outputData.IsList)
            {
                if (conversionRequired)
                {
                    //List conversion expression: (also add using System.Linq;)
                    //var variableName_afterConv = variableName_prevConv.Select(item => Converter.VarTypeToVarType(item)).ToList();

                    var itemConversionExpression = typeCon.ConversionCode_DGCommonToGh("item").ExpressionNode;
                    var listConversionExpression = CallMethodOnObject(variableName_preConv, "Select", Argument(LambdaExpr("item", itemConversionExpression))).CallMethodOnExpr("ToList");
                    var listConversionStatement = listConversionExpression.AssignToNewVariable(ListType(variableType), variableName_afterConv).ToStatement();
                    listConversionStatement = listConversionStatement.WithLeadingTrivia(CommentTriv(comment));

                    statementsToAppend.Add(listConversionStatement);
                }

                var daSetStatement = CallMethodOnObject("DA", "SetDataList", NumberArg(index), NamedArg(variableName_InDaSet)).ToStatement();
                if (!conversionRequired) daSetStatement = daSetStatement.WithLeadingTrivia(CommentTriv(comment));

                statementsToAppend.Add(daSetStatement);

            }
            else
            {
                if (conversionRequired)
                    InsertEvaluationStatements(conversionStatements, variableName_afterConv, statementsToAppend, comment, (variableName_preConv, variableType));

                var daSetStatement = CallMethodOnObject("DA", "SetData", NumberArg(index), NamedArg(variableName_InDaSet)).ToStatement();
                if (!conversionRequired) daSetStatement = daSetStatement.WithLeadingTrivia(CommentTriv(comment));

                statementsToAppend.Add(daSetStatement);
            }
        }

        /// <summary>
        /// Places the EvaluationStatements into the statements to append.
        /// This MAY involve creating a new method in the class for the Evaluation statement.
        /// </summary>
        private void InsertEvaluationStatements(EvaluationStatements evStatements, string variableName, List<StatementSyntax> statementsToAppend, string commentToInsert,
                                                        params (string name, TypeSyntax typeSyn)[] parameterPairs)
        {
            if (evStatements.CanCreateStatementsAsVariable)
            {
                var statementsToAdd = evStatements.CreateStatementsAsVariable(variableName);

                if (commentToInsert != null)
                    statementsToAdd[0] = statementsToAdd[0].WithLeadingTrivia(CommentTriv(commentToInsert));

                statementsToAppend.AddRange(statementsToAdd);
            }
            else
            {
                var methodName = $"Evaluate_{variableName}";

                var method = evStatements.CreateStatementsAsMethod(methodName, SyntaxKind.PrivateKeyword);
                if (parameterPairs.Length != 0) method = method.WithParameterList(ParamList(parameterPairs));
                ClassElements.Add(method);

                var callMethodStatement = CallMethod(methodName, parameterPairs.Select(pp => NamedArg(pp.name)).ToArray())
                                            .AssignToNewVariable(evStatements.ReturnType, variableName)
                                            .ToStatement();
                statementsToAppend.Add(callMethodStatement);
            }
        }

        ///// <summary>
        ///// Returns EvaluationStatements as an expression.
        ///// This MAY involve creating a new method in the class for the Evaluation statement.
        ///// </summary>
        //private void GetEvaluationStatementsAsExpression(EvaluationStatements evStatements, string variableName,
        //                                        params (string name, TypeSyntax typeSyn)[] parameterPairs)
        //{
        //    if (evStatements.CanCreateStatementsAsVariable)
        //        statementsToAppend.AddRange(evStatements.CreateStatementsAsVariable(variableName));
        //    else
        //    {
        //        var methodName = $"Evaluate_{variableName}";

        //        var method = evStatements.CreateStatementsAsMethod(methodName, SyntaxKind.PrivateKeyword);
        //        if (parameterPairs.Length != 0) method = method.WithParameterList(ParamList(parameterPairs));
        //        ClassElements.Add(method);

        //        var callMethodStatement = CallMethod(methodName, parameterPairs.Select(pp => NamedArg(pp.name)).ToArray())
        //                                    .AssignToNewVariable(evStatements.ReturnType, variableName)
        //                                    .ToStatement();
        //        statementsToAppend.Add(callMethodStatement);
        //    }
        //}

    }
}
