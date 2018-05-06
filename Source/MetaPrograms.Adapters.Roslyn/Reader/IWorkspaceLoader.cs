using Microsoft.CodeAnalysis;

namespace MetaPrograms.Adapters.Roslyn.Reader
{
    public interface IWorkspaceLoader
    {
        Workspace LoadProjectWorkspace(string projectFilePath);
    }
}