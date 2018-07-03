using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Example.AspNetAdapter;
using Example.HyperappAdapter;
using Example.WebUIModel;
using Example.WebUIModel.Metadata;
using MetaPrograms.Adapters.Roslyn;
using MetaPrograms.Adapters.Roslyn.Reader;
using MetaPrograms.CodeModel.Imperative;
using Microsoft.CodeAnalysis;
using NUnit.Framework;
using Shouldly;

namespace MetaPrograms.IntegrationTests.CSharpAndJavaScript
{
    [TestFixture]
    public class ExampleAppImplementationTests
    {
        [Test]
        public void CanCompileExampleProject()
        {
            // arrange
            
            var workspace = LoadExampleWebAppWorkspace();

            // act & assert
            
            workspace.CompileCodeOrThrow();
        }

        [Test]
        public void CanGenerateFrontEndImplementation()
        {
            // arrange
            
            var workspace = LoadExampleWebAppWorkspace();

            // act
            
            var codeModel = ReadCodeModel(workspace);
            var uiMetadata = new WebUIMetadata(codeModel);
            
            var output = new TestCodeGeneratorOutput();
            var adapter = new HyperappAdapter(output);
            adapter.GenerateImplementations(uiMetadata);

            // assert

            WriteOutputToDisk(output, "FrontEnd");
            AssertOutputs(
                output.SourceFiles, 
                "build/index.html", 
                "src/index.js",
                "src/components/form.js",
                "src/services/greetingService.js");
        }

        [Test]
        public void CanGenerateBackEndImplementation()
        {
            // arrange
            
            var workspace = LoadExampleWebAppWorkspace();

            // act
            
            var codeModel = ReadCodeModel(workspace);
            var uiMetadata = new WebUIMetadata(codeModel);
            
            var output = new TestCodeGeneratorOutput();
            var adapter = new AspNetAdapter(codeModel, output);
            adapter.GenerateImplementations(uiMetadata);

            // assert

            WriteOutputToDisk(output, "BackEnd");
            AssertOutputs(
                output.SourceFiles,
                "App/Services/WebApi/GreetingServiceController.cs",
                "App/Services/WebApi/GetGreetingForNameInvocation.cs",
                "AspNetAdapter/InvalidModelAutoResponderAttribute.cs");
        }

        private Workspace LoadExampleWebAppWorkspace()
        {
            var loader = new BuildalyzerWorkspaceLoader();
            var workspace = loader.LoadWorkspace(new[] {
                GetExampleProjectFilePath("Example.App"),
            });

            workspace.AddAssemblyReferences(new[] {
                typeof(WebUIMetadata).Assembly.Location
            });

            return workspace;
        }

        private ImperativeCodeModel ReadCodeModel(Workspace workspace)
        {
            var reader = new RoslynCodeModelReader(workspace);
            reader.Read();

            var codeModel = reader.GetCodeModel();
            return codeModel;
        }

        private void AssertOutputs(ImmutableDictionary<string, Stream> outputs, params string[] expectedFileNames)
        {
            outputs
                .Select(kvp => kvp.Key)
                .ShouldBe(expectedFileNames, ignoreOrder: true);

            var expectedOutputsDirectory = Path.Combine(ExamplesRootDirectory, "ExpectedOutput");
            
            foreach (var fileName in expectedFileNames)
            {
                outputs[fileName].ShouldMatchTextFile(Path.Combine(expectedOutputsDirectory, fileName));
            }
        }

        private void WriteOutputToDisk(TestCodeGeneratorOutput output, string subFolder)
        {
            var outputPath = Path.Combine(@"C:\temp\codegen", subFolder);
            
            foreach (var pair in output.SourceFiles)
            {
                var filePath = Path.Combine(outputPath, pair.Key);
                var folderPath = Path.GetDirectoryName(filePath);

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                using (var file = File.Create(filePath))
                {
                    pair.Value.Position = 0;
                    pair.Value.CopyTo(file);
                    file.Flush();
                }
            }
        }

        private string GetExampleProjectFilePath(string projectName) =>
            Path.Combine(ExamplesRootDirectory, projectName, projectName + ".csproj");

        private string ExamplesRootDirectory
        {
            get
            {
                return Path.Combine(
                    TestContext.CurrentContext.TestDirectory,
                    "..",
                    "..",
                    "..",
                    "..",
                    "CSharpAndJavaScript"
                );
            }
        }
    }
}
