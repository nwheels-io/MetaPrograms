using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Buildalyzer;
using Buildalyzer.Environment;
using Buildalyzer.Workspaces;
using MetaPrograms.CodeModel.Imperative.Members;
using Microsoft.CodeAnalysis;

namespace MetaPrograms.Adapters.Roslyn.Reader
{
    public class BuildalyzerWorkspaceLoader : IWorkspaceLoader
    {
        private readonly AnalyzerManager _analyzerManager = new AnalyzerManager();

        public Workspace LoadWorkspace(IEnumerable<string> projectFilePaths)
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

