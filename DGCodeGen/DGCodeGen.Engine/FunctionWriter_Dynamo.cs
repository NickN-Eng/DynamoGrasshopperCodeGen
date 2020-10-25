using DGCodeGen.TypeConversions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static DGCodeGen.Engine.SyntaxHelper;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DGCodeGen.Engine
{
    public class FunctionWriter_Dynamo
    {
        private App App;

        public List<UsingDirectiveSyntax> Usings;

        //A temp variable which is reset for every function generation
        private List<MemberDeclarationSyntax> ClassElements;

        public FunctionWriter_Dynamo(App app)
        {
            App = app;

            //Create default usings for test
            Usings = new List<UsingDirectiveSyntax>
            {
                UsingDirective(IdentifierName("System")),
                UsingDirective(AutoChainedName("System.Collections.Generic")),
                UsingDirective(AutoChainedName("System.Linq")),
                UsingDirective(AutoChainedName("Autodesk.DesignScript.Runtime"))
            };
        }

        public static void QuickWriteAll(App app, FunctionCollection functionCollection)
        {
            var functionWriter = new FunctionWriter_Dynamo(app);
            functionWriter.WriteAll(functionCollection);
        }

        public void WriteAll(FunctionCollection functions)
        {
            if (functions.Functions.Count == 0) return;

            var generatedNodeFilepath = App.FileConfig.DynamoProjectFilepath + @"\DGCodeGen\";
            Directory.CreateDirectory(generatedNodeFilepath);

            var funcsByParent = functions.Functions.Where(f => f.DynamoFunctionAttr != null).GroupBy(f => f.DynamoFunctionAttr.Parent);

            foreach (IGrouping<string, FunctionData> funcGroup in funcsByParent)
            {
                var text = Generate(funcGroup, funcGroup.Key).ToFullString();

                File.WriteAllText($"{generatedNodeFilepath}{funcGroup.Key}.cs", text);
            }
        }

        public CompilationUnitSyntax Generate(IEnumerable<FunctionData> functionData, string className)
        {
            ClassElements = new List<MemberDeclarationSyntax>();

            foreach (var func in functionData)
            {
                var (inputParams, inputConversionStatements) = InputParameters(func);
                var (returnType, returnStatement, returnConversionStatements, multireturnAttr) = ReturnType(func);

                var bodyStatements = new List<StatementSyntax>();
                
                //Inputs
                bodyStatements.AddRange(inputConversionStatements);

                //Method body
                if (func.FunctionBody.IsTupleReturn)
                    bodyStatements.AddRange(func.FunctionBody.CreateStatementsAsTupleVariables(func.Outputs.Select(o => o.ParameterName).ToArray()));
                else
                    bodyStatements.AddRange(func.FunctionBody.CreateStatementsAsVariable(func.Outputs[0].ParameterName));

                //Output
                bodyStatements.AddRange(returnConversionStatements);
                bodyStatements.Add(returnStatement);

                var meth = MethodDeclaration(returnType, func.DynamoFunctionAttr.Name)
                            .WithModifiers(TokenList(new[] { Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword) }))
                            .WithParameterList(inputParams)
                            .WithBody(Block(bodyStatements));

                if (multireturnAttr != null)
                    meth = meth.WithAttributeLists(SingletonList(multireturnAttr));

                ClassElements.Add(meth);
            }

            CompilationUnitSyntax result = CompilationUnit()
                    .WithUsings(List(Usings))
                    .WithMembers(SingletonList<MemberDeclarationSyntax>(GetNamespaceFromString(App.FileConfig.DynamoNamespace)
                        .WithMembers(
                            SingletonList<MemberDeclarationSyntax>(
                                ClassDeclaration(className) //Class name
                                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword))) //public class
                                .WithMembers(List(ClassElements))))));

            result = result.NormalizeWhitespace();
            return result;
        }

        private (ParameterListSyntax inputParams, List<StatementSyntax> conversionStatements) InputParameters(FunctionData functionData)
        {
            //Support for lists TBC!!
            if (functionData.Outputs.Count == 0)
            {
                return (ParameterList(), new List<StatementSyntax>());
            }
            else
            {
                var noInputs = functionData.Inputs.Count;
                var paramPairs = new (string varName, string varType)[noInputs];
                List<StatementSyntax> conversionStatement = new List<StatementSyntax>();

                for (int i = 0; i < noInputs; i++)
                {
                    var iInput = functionData.Inputs[i];
                    var iTypeConverter = App.TypeDictionary.Get(iInput.DGCommonType); //Group up types???

                    var iParameterName = iInput.InputAttr.Name;
                    var iConversion = iTypeConverter.ConversionCode_DGCommonToDy(iParameterName);
                    if (iConversion != null)
                    {
                        //If the input.InputAttr name matches the parameter name,
                        //need to rename the input attribute name so that methodBodyParameterName = Convert(inputParameterName)
                        if (iParameterName == iInput.ParameterName)
                        {
                            if (char.IsUpper(iParameterName[0]))
                                iParameterName = iParameterName.FirstCharToLower();
                            else
                                iParameterName = iParameterName.FirstCharToUpper();

                            iConversion = iTypeConverter.ConversionCode_DGCommonToDy(iParameterName);
                        }

                        conversionStatement.AddRange(iConversion.CreateStatementsAsVariable(iInput.ParameterName));
                    }
                    else
                    {
                        //It the desired Node Input name is not the same as the internal method parameter name, use a simple assignment statement to declare it so.
                        if (iParameterName != iInput.ParameterName)
                        {
                            //write a statement: var [iInput.ParameterName] = [iInput.InputAttr.Name];
                            var assignmentStatement = LocalDeclarationStatement(VariableDeclaration(IdentifierName("var"))
                                                .WithVariables(SingletonSeparatedList(VariableDeclarator(Identifier(iInput.ParameterName)) 
                                                    .WithInitializer(EqualsValueClause(IdentifierName(iParameterName)))))); 
                            conversionStatement.Add(assignmentStatement);
                        }
                    }

                    paramPairs[i] = (iParameterName, iTypeConverter.DynamoTypeName);
                }

                return (ParamList(paramPairs), conversionStatement);
            }
        }

        private (TypeSyntax returnType, ReturnStatementSyntax returnStatement, List<StatementSyntax> conversionStatements, AttributeListSyntax multireturnAttr) ReturnType(FunctionData functionData)
        {
            //Support for lists TBC!!
            if (functionData.Outputs.Count == 0)
            {
                return (PredefType(SyntaxKind.VoidKeyword), null, null, null);
            }
            if (functionData.Outputs.Count == 1)
            {
                var output = functionData.Outputs[0];
                TypeConversion typeConversion = App.TypeDictionary.Get(output.DGCommonType);

                var conversion = typeConversion.ConversionCode_DGCommonToDy(output.ParameterName);
                List<StatementSyntax> conversionStatement;
                string returnVariableName;
                if (conversion != null)
                {
                    returnVariableName = output.ParameterName + "_converted";
                    conversionStatement = conversion.CreateStatementsAsVariable(returnVariableName);
                }
                else
                {
                    returnVariableName = output.ParameterName;
                    conversionStatement = new List<StatementSyntax>();
                }

                return (NamedType(typeConversion.DynamoTypeName), ReturnStatement(IdentifierName(returnVariableName)), conversionStatement, null);
            }
            else
            {
                var keyType = PredefType(SyntaxKind.StringKeyword);
                var valueType = PredefType(SyntaxKind.ObjectKeyword);
                var dictType = DictType(keyType, valueType);

                var noOutputs = functionData.Outputs.Count;
                var dictPairs = new (ExpressionSyntax keyExpr, ExpressionSyntax valueExpr)[noOutputs];
                List<StatementSyntax> conversionStatement = new List<StatementSyntax>();
                for (int i = 0; i < noOutputs; i++)
                {
                    var iOutput = functionData.Outputs[i];
                    var iTypeConverter = App.TypeDictionary.Get(iOutput.DGCommonType); //Group up types???

                    var iConversion = iTypeConverter.ConversionCode_DGCommonToDy(iOutput.ParameterName);
                    var iReturnVariableName = iOutput.ParameterName;
                    if (iConversion != null)
                    {
                        iReturnVariableName += "_converted";
                        conversionStatement.AddRange(iConversion.CreateStatementsAsVariable(iReturnVariableName));
                    }

                    dictPairs[i] = (StringExpr(iOutput.OutputAttr.Name), IdentifierName(iOutput.ParameterName));
                }
                var returnStatement = ReturnStatement(CreateNewDictionary(keyType, valueType, dictPairs));

                var multiReturnAttr = AttributeWithArgs("MultiReturn", functionData.Outputs.Select(o => AttrStringLitArg(o.OutputAttr.Name)).ToArray());

                return (dictType, returnStatement, conversionStatement, multiReturnAttr);
            }
        }
    }
}