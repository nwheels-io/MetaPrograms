using System.Collections.Generic;
using Buildalyzer;
using Buildalyzer.Workspaces;
using Microsoft.CodeAnalysis;

namespace MetaPrograms.Adapters.Roslyn.Reader
{
    public class BuildalyzerWorkspaceLoader : IWorkspaceLoader
    {
        private readonly AnalyzerManager _analyzerManager = new AnalyzerManager();

        public Workspace LoadProjectWorkspace(IEnumerable<string> projectFilePaths)
        {
            var workspace = new AdhocWorkspace();

            foreach (var filePath in projectFilePaths)
            {
                ProjectAnalyzer analyzer = _analyzerManager.GetProject(filePath);
                analyzer.AddToWorkspace(workspace);
            }

            return workspace;
        }
    }
}

