using System;
using System.Collections.Generic;
using Buildalyzer;
using Buildalyzer.Workspaces;
using MetaPrograms.CodeModel.Imperative;
using MetaPrograms.CodeModel.Imperative.Members;
using Microsoft.CodeAnalysis;

namespace MetaPrograms.Adapters.Roslyn
{
    public class RoslynCodeModelReader
    {
        private readonly AnalyzerManager _buildalyzer = new AnalyzerManager();
        private readonly List<TypeMember> _types = new List<TypeMember>();
        
        public void AddProject(string projectFilePath)
        {
            ProjectAnalyzer analyzer = _buildalyzer.GetProject(projectFilePath);
            AdhocWorkspace workspace = analyzer.GetWorkspace();         
        }

        public void Read()
        {
            
            
            
            TypeMembers = new CodeMemberCollection(_types); 
        }

        public CodeMemberCollection TypeMembers { get; private set; }
    }
}

