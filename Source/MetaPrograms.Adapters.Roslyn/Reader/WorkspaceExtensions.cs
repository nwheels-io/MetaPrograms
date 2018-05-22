using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Schema;
using Microsoft.CodeAnalysis;

namespace MetaPrograms.Adapters.Roslyn.Reader
{
    public static class WorkspaceExtensions
    {
        public static Compilation CompileCodeOrThrow(this Workspace workspace)
        {
            var project = workspace.CurrentSolution.Projects.First();
            var compilation = project.GetCompilationAsync().Result;
            var allDiagnostics = compilation.GetDiagnostics();
            var warningsAndErrors = compilation
                .GetDiagnostics()
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
    }
}
