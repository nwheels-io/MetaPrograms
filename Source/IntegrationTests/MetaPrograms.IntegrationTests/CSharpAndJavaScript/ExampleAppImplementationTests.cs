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
            var workspace = loader.LoadProjectWorkspace(new[] {
                GetExampleProjectFilePath("Example.App"),
                GetExampleProjectFilePath("Example.WebUIModel"),
                GetExampleProjectFilePath("Example.WebBrowserAdapter"),
                GetExampleProjectFilePath("Example.AspNetAdapter"),
            });
                    ;

            //act & assert
            workspace.CompileCodeOrThrow();
        }

        [Test]
        public void CanGenerateImplementations()
        {
            // act

            var reader = new RoslynCodeModelReader(new BuildalyzerWorkspaceLoader());
            reader.AddProject(GetExampleProjectFilePath("Example.App"));
            reader.AddProject(GetExampleProjectFilePath("Example.WebUIModel"));
            reader.AddProject(GetExampleProjectFilePath("Example.WebBrowserAdapter"));
            reader.AddProject(GetExampleProjectFilePath("Example.AspNetAdapter"));
            reader.Read();
            
            var uiMetadata = new WebUIMetadata(reader.CodeModel);
            
            var frontEndAdapter = new WebBrowserAdapter(outputStreamFactory: filePath => new MemoryStream());
            var frontEndOutputs = frontEndAdapter.GenerateImplementations(uiMetadata);
            
            var backEndAdapter = new AspNetAdapter(outputStreamFactory: filePath => new MemoryStream());
            var backEndOutputs = backEndAdapter.GenerateImplementations(uiMetadata);

            // assert

            AssertOutputs(frontEndOutputs, "index.html", "index.js", "tinyfx.js");
            AssertOutputs(backEndOutputs, "IndexController.cs", "InvalidModelAutoResponderAttribute.cs");
        }

        private void AssertOutputs(ImmutableDictionary<string, Stream> outputs, params string[] expectedFileNames)
        {
            outputs.Select(kvp => kvp.Key).ShouldBe(expectedFileNames, ignoreOrder: true);

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
