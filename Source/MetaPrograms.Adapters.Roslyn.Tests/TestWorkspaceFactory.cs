using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace MetaPrograms.Adapters.Roslyn.Tests
{
    public static class TestWorkspaceFactory
    {
        private static readonly IReadOnlyList<string> TrustedAssemblyPaths = 
            AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES").ToString().Split(';');

        public static AdhocWorkspace CreatreDefaultWithCode(string csharpCode, out Project project)
        {
            var sourceText = SourceText.From(csharpCode);
                
            var workspace = new AdhocWorkspace();
            var projectId = ProjectId.CreateNewId();
            var documentInfo = DocumentInfo.Create(
                DocumentId.CreateNewId(projectId, "MyClass.cs"),
                "MyClass.cs",
                loader: TextLoader.From(TextAndVersion.Create(sourceText, VersionStamp.Create())));

            var projectInfo = ProjectInfo
                .Create(projectId, VersionStamp.Create(), "MyTest", "MyTest", LanguageNames.CSharp)
                .WithCompilationOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .WithMetadataReferences(GetSystemAssemblyPaths().Select(path => MetadataReference.CreateFromFile(path)))
                .WithDocuments(new[] {documentInfo});
            
            project = workspace.AddProject(projectInfo);
            return workspace;
        }
        
        private static string[] GetSystemAssemblyPaths(params string[] additionalAssemblyNames)
        {
            var assemblyNameSet = new HashSet<string>(additionalAssemblyNames.Select(name => name + ".dll"));

            assemblyNameSet.Add("mscorlib.dll");
            assemblyNameSet.Add("System.Runtime.dll");
            assemblyNameSet.Add("System.Private.CoreLib.dll");
            assemblyNameSet.Add("System.Private.Library.dll");

            var selectedAssemblyPaths = TrustedAssemblyPaths
                .Where(path => assemblyNameSet.Contains(Path.GetFileName(path)))
                .ToArray();

            return selectedAssemblyPaths;
        }
    }
}