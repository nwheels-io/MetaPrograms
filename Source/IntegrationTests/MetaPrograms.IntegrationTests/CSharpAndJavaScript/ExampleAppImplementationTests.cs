using System.IO;
using System.Linq;
using Example.AspNetAdapter;
using Example.WebBrowserAdapter;
using Example.WebUIModel;
using MetaPrograms.Adapters.Roslyn;
using NUnit.Framework;
using Shouldly;

namespace MetaPrograms.IntegrationTests.CSharpAndJavaScript
{
    [TestFixture]
    public class ExampleAppImplementationTests
    {
        [Test]
        public void CanGenerateImplementations()
        {
            // act

            var reader = new RoslynCodeModelReader();
            reader.AddProject(Path.Combine(ExamplesRootDirectory, "Example.App", "Example.App.csproj"));
            reader.Read();
            
            var uiModel = new WebUIModel(reader.TypeMembers);
            
            var frontEndAdapter = new WebBrowserAdapter(outputStreamFactory: filePath => new MemoryStream());
            var frontEndOutputs = frontEndAdapter.GenerateImplementations(uiModel);
            
            var backEndAdapter = new AspNetAdapter(outputStreamFactory: filePath => new MemoryStream());
            var backEndOutputs = backEndAdapter.GenerateImplementations(uiModel);
            
            // assert

            frontEndOutputs.Select(kvp => kvp.Key).ShouldBe(new[] { "index.html", "index.js" }, ignoreOrder: true);
            backEndOutputs.Select(kvp => kvp.Key).ShouldBe(new[] { "IndexController.cs" });

            var expectedOutputsDirectory = Path.Combine(ExamplesRootDirectory, "ExpectedOutput");

            frontEndOutputs["index.html"].ShouldMatchTextFile(Path.Combine(expectedOutputsDirectory, "index.html"));
            frontEndOutputs["index.js"].ShouldMatchTextFile(Path.Combine(expectedOutputsDirectory, "index.js"));
            backEndOutputs["IndexController.cs"].ShouldMatchTextFile(Path.Combine(expectedOutputsDirectory, "IndexController.cs"));
        }

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
