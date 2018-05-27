using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Example.AspNetAdapter;
using Example.WebBrowserAdapter;
using Example.WebUIModel;
using Example.WebUIModel.Metadata;
using MetaPrograms.Adapters.Roslyn;
using MetaPrograms.Adapters.Roslyn.Reader;
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
            
            var loader = new BuildalyzerWorkspaceLoader();
            var workspace = loader.LoadWorkspace(new[] {
                GetExampleProjectFilePath("Example.App"),
            });
            workspace.AddAssemblyReferences(new[] {
                typeof(WebUIMetadata).Assembly.Location
            });

            // act & assert
            
            workspace.CompileCodeOrThrow();
        }

        [Test]
        public void CanGenerateImplementations()
        {
            // arrange
            
            var loader = new BuildalyzerWorkspaceLoader();
            var workspace = loader.LoadWorkspace(new[] {
                GetExampleProjectFilePath("Example.App"),
            });
            workspace.AddAssemblyReferences(new[] {
                typeof(WebUIMetadata).Assembly.Location
            });

            // act
            
            var reader = new RoslynCodeModelReader(workspace);
            reader.Read();

            var codeModel = reader.GetCodeModel();
            var uiMetadata = new WebUIMetadata(codeModel);
            
            var frontEndOutput = new TestCodeGeneratorOutput();
            var frontEndAdapter = new WebBrowserAdapter(frontEndOutput);
            frontEndAdapter.GenerateImplementations(uiMetadata);
            
            var backEndOutput = new TestCodeGeneratorOutput();
            var backEndAdapter = new AspNetAdapter(backEndOutput);
            backEndAdapter.GenerateImplementations(uiMetadata);

            // assert

            AssertOutputs(frontEndOutput.SourceFiles, "index.html", "index.js", "tinyfx.js");
            AssertOutputs(backEndOutput.SourceFiles, "IndexController.cs", "InvalidModelAutoResponderAttribute.cs");
        }

        private void AssertOutputs(ImmutableDictionary<string, Stream> outputs, params string[] expectedFileNames)
        {
            outputs
                .Select(kvp => kvp.Key)
                .Select(Path.GetFileName)
                .ShouldBe(expectedFileNames, ignoreOrder: true);

            var expectedOutputsDirectory = Path.Combine(ExamplesRootDirectory, "ExpectedOutput");
            
            foreach (var fileName in expectedFileNames)
            {
                outputs[fileName].ShouldMatchTextFile(Path.Combine(expectedOutputsDirectory, fileName));
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
