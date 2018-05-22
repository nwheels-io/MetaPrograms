using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MetaPrograms.CodeModel.Imperative;
using MetaPrograms.CodeModel.Imperative.Members;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MetaPrograms.Adapters.Roslyn.Reader
{
    public class RoslynCodeModelReader
    {
        private readonly List<IPhasedTypeReader> _phasedTypeReaders = new List<IPhasedTypeReader>();

        public RoslynCodeModelReader(Workspace workspace)
        {
            this.Workspace = workspace;
            this.ModelBuilder = new CodeModelBuilder();
        }

        public void Read()
        {
            DiscoverAllTypes();
            ReadAllTypes();
        }

        public ImmutableCodeModel GetCodeModel() => ModelBuilder.GetCodeModel();

        public Workspace Workspace { get; }
        public CodeModelBuilder ModelBuilder { get; }

        private void DiscoverAllTypes()
        {
            foreach (var project in Workspace.CurrentSolution.Projects)
            {
                var compilation = CompileProjectOrThrow(project);
                var visitor = new TypeDiscoverySymbolVisitor(compilation, ModelBuilder, _phasedTypeReaders);
                compilation.GlobalNamespace.Accept(visitor);
            }
        }

        private void ReadAllTypes()
        {
            _phasedTypeReaders.RunAllPhases();
        }

        private Compilation CompileProjectOrThrow(Project project)
        {
            var compilation = project.GetCompilationAsync().Result;
            var allDiagnostics = compilation.GetDiagnostics();
            var warningsAndErrors = compilation
                .GetDiagnostics()
                .Where(d => d.Severity >= DiagnosticSeverity.Warning)
                .ToArray();

            if (warningsAndErrors.Length > 0)
            {
                throw new Exception(GetDiagnosticsText(warningsAndErrors));
            }

            return compilation;
        }

        private string GetDiagnosticsText(Diagnostic[] warningsAndErrors)
        {
            var endl = Environment.NewLine;
            var messages = string.Join(endl, warningsAndErrors.Select(x => x.ToString()));
            var text = $"Project failed to compile.{endl}{messages}";

            return text;
        }
    }
}

