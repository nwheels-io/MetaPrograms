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
        private readonly List<Workspace> _projectWorkspaces = new List<Workspace>();
        private readonly CodeModelBuilder _modelBuilder = new CodeModelBuilder();
        
        public RoslynCodeModelReader(IWorkspaceLoader workspaceLoader)
        {
            _workspaceLoader = workspaceLoader;
        }

        public void AddProject(string projectFilePath)
        {
            var workspace = _workspaceLoader.LoadProjectWorkspace(projectFilePath);
            _projectWorkspaces.Add(workspace);
        }

        public void Read()
        {
            foreach (var workspace in _projectWorkspaces)
            {
                var project = workspace.CurrentSolution.Projects.Single();
                var projectReader = new ProjectReader(_modelBuilder, workspace, project);
                projectReader.Read();
            }

            CodeModel = _modelBuilder.GetCodeModel();
        }

        public ImmutableCodeModel CodeModel { get; private set; }
    }
}

