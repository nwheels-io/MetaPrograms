using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using MetaPrograms.Adapters.Roslyn.Reader;

namespace MetaPrograms.Adapters.Roslyn.Tests
{
    public static class TestWorkspaceFactory
    {
        private static readonly IReadOnlyList<string> TrustedAssemblyPaths = 
            AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES").ToString().Split(';');

        public static AdhocWorkspace CreateWithCode(string csharpCode, out Project project)
        {
            return CreateWithCode(csharpCode, new string[0], out project);
        }

        public static AdhocWorkspace CreateWithCode(string csharpCode, string[] references)
        {
            return CreateWithCode(csharpCode, references, out Project project);
        }

        public static AdhocWorkspace CreateWithCode(string csharpCode, string[] references, out Project project)
        {
            var sourceText = SourceText.From(csharpCode);
                
            var workspace = new AdhocWorkspace();
            var projectId = ProjectId.CreateNewId();
            var documentInfo = DocumentInfo.Create(
                DocumentId.CreateNewId(projectId, "Source.cs"),
                name: "Source.cs",
                loader: TextLoader.From(TextAndVersion.Create(sourceText, VersionStamp.Create())));

            var allReferencedAssemblyPaths = GetSystemAssemblyPaths().Concat(references);
            
            var projectInfo = ProjectInfo
                .Create(projectId, VersionStamp.Create(), "Test", "Test", LanguageNames.CSharp)
                .WithCompilationOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .WithMetadataReferences(allReferencedAssemblyPaths.Select(path => MetadataReference.CreateFromFile(path)))
                .WithDocuments(new[] { documentInfo });
            
            project = workspace.AddProject(projectInfo);
            return workspace;
        }

        public static Compilation CompileCodeOrThrow(string csharpCode, params string[] references)
        {
            var workspace = CreateWithCode(csharpCode, references, out Project project);
            return workspace.CompileCodeOrThrow();
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