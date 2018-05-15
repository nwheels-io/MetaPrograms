using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace MetaPrograms.Adapters.Roslyn.Reader
{
    public interface IWorkspaceLoader
    {
        Workspace LoadWorkspace(IEnumerable<string> projectFilePaths);
    }
}