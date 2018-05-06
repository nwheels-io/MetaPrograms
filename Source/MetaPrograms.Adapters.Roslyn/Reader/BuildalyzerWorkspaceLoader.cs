using Buildalyzer;
using Buildalyzer.Workspaces;
using Microsoft.CodeAnalysis;

namespace MetaPrograms.Adapters.Roslyn.Reader
{
    public class BuildalyzerWorkspaceLoader : IWorkspaceLoader
    {
        private readonly AnalyzerManager _analyzerManager = new AnalyzerManager();

        public Workspace LoadProjectWorkspace(string projectFilePath)
        {
            ProjectAnalyzer analyzer = _analyzerManager.GetProject(projectFilePath);
            AdhocWorkspace workspace = analyzer.GetWorkspace();
            return workspace;
        }
    }
}

