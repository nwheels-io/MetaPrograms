using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Schema;
using Microsoft.CodeAnalysis;

namespace MetaPrograms.CSharp.Reader
{
    public static class WorkspaceExtensions
    {
        public static Compilation CompileCodeOrThrow(this Workspace workspace)
        {
            var project = workspace.CurrentSolution.Projects
                .First()
                .WithoutInvalidDocuments();

            var compilation = project.GetCompilationAsync().Result;
            var allDiagnostics = compilation.GetDiagnostics();
            var warningsAndErrors = allDiagnostics
                .Where(d => d.Severity >= DiagnosticSeverity.Warning)
                .ToArray();

            if (warningsAndErrors.Length > 0)
            {
                var endl = Environment.NewLine;
                var diagnosticsText = string.Join(endl, warningsAndErrors.Select(x => x.ToString()));
                throw new Exception($"Code failed to compile{endl}{diagnosticsText}");
            }

            return compilation;
        }

        public static void AddAssemblyReferences(this Workspace workspace, IEnumerable<string> assemblyReferencePaths)
        {
            var newSolution = workspace.CurrentSolution;
            var assemblyReferences = assemblyReferencePaths.Select(path => MetadataReference.CreateFromFile(path)).ToArray();

            foreach (var projectId in workspace.CurrentSolution.ProjectIds)
            {
                foreach (var reference in assemblyReferences)
                {
                    newSolution = newSolution.AddMetadataReference(projectId, reference);
                }
            }

            if (!workspace.TryApplyChanges(newSolution))
            {
                throw new InvalidOperationException("Failed to set project references");
            }
        }

        public static Project WithoutInvalidDocuments(this Project project)
        {
            var badDocumentIds = project.Documents
                .Where(doc => doc.FilePath?.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase) == true)
                .Select(doc => doc.Id)
                .ToArray();

            foreach (var id in badDocumentIds)
            {
                project = project.RemoveDocument(id);
            }

            return project;
        }
    }
}
