using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Example.AspNetAdapter;
using Example.HyperappAdapter;
using Example.HyperappAdapter.Components;
using Example.WebUIModel;
using Example.WebUIModel.Metadata;
using MetaPrograms.CSharp;
using MetaPrograms.CSharp.Reader;
using MetaPrograms;
using MetaPrograms.CSharp.Reader.Reflection;
using MetaPrograms.Testability;
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
            var adapter = new HyperappAdapter(codeModel, new ComponentAdapterFactory(), output);
            adapter.GenerateImplementations(uiMetadata);
            
            // assert
            
            //WriteOutputToDisk(output, "FrontEnd");
            AssertOutputs(
                output.IndexSourceFilesByNormalPath(),
                subFolder: "FrontEnd",
                "build/index.html", 
                "src/index.js",
                "src/components/form.js",
                "src/services/greeting-service.js");
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

            //WriteOutputToDisk(output, "BackEnd");
            AssertOutputs(
                output.IndexSourceFilesByNormalPath(),
                subFolder: "BackEnd",
                "AspNetAdapter/InvalidModelAutoResponderAttribute.cs",
                "App/Services/WebApi/GreetingServiceController.cs");
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
            var reader = new RoslynCodeModelReader(workspace, new ClrTypeResolver());
            reader.Read();

            var codeModel = reader.GetCodeModel();
            return codeModel;
        }

        private void AssertOutputs(IDictionary<string, Stream> actualOutputs, string subFolder, params string[] expectedFileNames)
        {
            actualOutputs
                .Select(kvp => kvp.Key)
                .ShouldBe(expectedFileNames, ignoreOrder: true);

            var expectedOutputsDirectory = Path.Combine(ExamplesRootDirectory, "..", "ExpectedOutput", subFolder);
            
            foreach (var fileName in expectedFileNames)
            {
                var actualStream = actualOutputs[fileName];
                actualStream.Position = 0;
                actualStream.ShouldMatchTextFile(Path.Combine(expectedOutputsDirectory, fileName));
            }
        }

        private void WriteOutputToDisk(TestCodeGeneratorOutput output, string subFolder)
        {
            var outputPath = Path.Combine(@"C:\temp\codegen", subFolder);
            
            foreach (var pair in output.IndexSourceFilesByNormalPath())
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
                    "..",
                    "Demos",
                    "01-cs-and-js",
                    "Source"
                );
            }
        }
    }
}
