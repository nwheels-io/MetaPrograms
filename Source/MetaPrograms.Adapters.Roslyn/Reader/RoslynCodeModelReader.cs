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
        private readonly List<IPhasedTypeReader> _phasedTypeReaders;

        public RoslynCodeModelReader(Workspace workspace)
        {
            this.Workspace = workspace;
        }

        public void Read()
        {
            DiscoverAllTypes();

            //foreach (var project in _workspace.CurrentSolution.Projects)
            //{
            //    var projectReader = new ProjectReader(_modelBuilder, _workspace, project);
            //    projectReader.Read();
            //}

            //CodeModel = _modelBuilder.GetCodeModel();
        }

        public ImmutableCodeModel GetCodeModel() => ModelBuilder.GetCodeModel();

        public Workspace Workspace { get; }
        public CodeModelBuilder ModelBuilder { get; }

        private void DiscoverAllTypes()
        {
            throw new System.NotImplementedException();
        }
    }
}

