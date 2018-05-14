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
        private readonly IWorkspaceLoader _workspaceLoader;
        private readonly List<string> _projectFilePaths = new List<string>();
        private readonly CodeModelBuilder _modelBuilder = new CodeModelBuilder();
        private Workspace _workspace = null;
        
        public RoslynCodeModelReader(IWorkspaceLoader workspaceLoader)
        {
            _workspaceLoader = workspaceLoader;
        }

        public void AddProject(string projectFilePath)
        {
            _projectFilePaths.Add(projectFilePath);
        }

        public void Read()
        {
            _workspace = _workspaceLoader.LoadProjectWorkspace(_projectFilePaths);

            foreach (var project in _workspace.CurrentSolution.Projects)
            {
                var projectReader = new ProjectReader(_modelBuilder, _workspace, project);
                projectReader.Read();
            }

            CodeModel = _modelBuilder.GetCodeModel();
        }

        public ImmutableCodeModel CodeModel { get; private set; }
    }
}

