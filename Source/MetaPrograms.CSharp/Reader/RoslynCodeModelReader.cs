using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using MetaPrograms;
using MetaPrograms.Members;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MetaPrograms.CSharp.Reader
{
    public class RoslynCodeModelReader
    {
        private readonly List<IPhasedTypeReader> _phasedTypeReaders = new List<IPhasedTypeReader>();
        private readonly IClrTypeResolver _clrTypeResolver;

        public RoslynCodeModelReader(Workspace workspace, IClrTypeResolver clrTypeResolver)
        {
            _clrTypeResolver = clrTypeResolver;
            
            this.Workspace = workspace;
            this.Compilations = CompileAllProjects(Workspace);
            this.ModelBuilder = new CodeModelBuilder(Compilations);
        }

        public RoslynCodeModelReader(Compilation compilation, IClrTypeResolver clrTypeResolver)
        {
            _clrTypeResolver = clrTypeResolver;
            
            this.Workspace = null;
            this.Compilations = new[] { compilation };
            this.ModelBuilder = new CodeModelBuilder(Compilations);
        }

        public void Read()
        {
            using (var context = new CodeReaderContext(ModelBuilder.GetCodeModel(), _clrTypeResolver, LanguageInfo.Entries.CSharp()))
            {
                DiscoverAllTypes();
                ReadAllTypes();
            }
        }
        
        public PortableExecutableReference TryGetAssemblyPEReference(TypeMember type)
        {
            var typeSymbol = type.GetBindingOrThrow<INamedTypeSymbol>();
            PortableExecutableReference reference;

            if (typeSymbol.ContainingAssembly != null)
            {
                reference = Compilations
                    .Select(x => x.GetMetadataReference(typeSymbol.ContainingAssembly))
                    .OfType<PortableExecutableReference>()
                    .FirstOrDefault();
            }
            else
            {
                reference = null;
            }

            return reference;
        }

        public ImperativeCodeModel GetCodeModel() => ModelBuilder.GetCodeModel();
        public Workspace Workspace { get; }
        public CodeModelBuilder ModelBuilder { get; private set; }
        public IEnumerable<Compilation> Compilations { get; }

        private void DiscoverAllTypes()
        {
            foreach (var compilation in ModelBuilder.GetCompilations())
            {
                var visitor = new TypeDiscoverySymbolVisitor(ModelBuilder, _phasedTypeReaders);
                compilation.GlobalNamespace.Accept(visitor);
            }
        }

        private void ReadAllTypes()
        {
            _phasedTypeReaders.RunAllPhases();
        }

        private static IEnumerable<Compilation> CompileAllProjects(Workspace workspace)
        {
            foreach (var project in workspace.CurrentSolution.Projects)
            {
                yield return CompileProjectOrThrow(project.WithoutInvalidDocuments());
            }
        }

        private static Compilation CompileProjectOrThrow(Project project)
        {
            var compilation = project.GetCompilationAsync().Result;
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

        private static string GetDiagnosticsText(Diagnostic[] warningsAndErrors)
        {
            var endl = Environment.NewLine;
            var messages = string.Join(endl, warningsAndErrors.Select(x => x.ToString()));
            var text = $"Project failed to compile.{endl}{messages}";

            return text;
        }
    }
}
