using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Buildalyzer;
using Buildalyzer.Workspaces;
using MetaPrograms.CodeModel.Imperative;
using MetaPrograms.CodeModel.Imperative.Members;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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

            var solution = workspace.CurrentSolution;
            var projects = solution.Projects.ToArray();
            var firstProject = projects[0];
            var compilation = firstProject.GetCompilationAsync(CancellationToken.None).Result;
            var syntaxTrees = compilation.SyntaxTrees.ToArray();

            foreach (var tree in syntaxTrees)
            {
                var semanticModel = compilation.GetSemanticModel(tree);

                var topLevelClasses = tree.GetCompilationUnitRoot()
                    .DescendantNodes(descendIntoChildren: node => {
                        var result = (node is CompilationUnitSyntax || node is NamespaceDeclarationSyntax);
                        return result;
                    })
                    .OfType<ClassDeclarationSyntax>()
                    .ToList();

                foreach (var singleClass in topLevelClasses)
                {
                    _types.Add(ParseClass(semanticModel, singleClass));
                }
            }
        }

        private TypeMember ParseClass(SemanticModel semanticModel, ClassDeclarationSyntax syntax)
        {
            var typeSymbol = semanticModel.GetDeclaredSymbol(syntax);
            var builder = new TypeMemberBuilder();
            builder.Namespace = typeSymbol.ContainingNamespace?.Name;
            builder.Name = typeSymbol.Name;
            return new TypeMember(builder);
        }

        public void Read()
        {
            
            
            
            TypeMembers = new CodeMemberCollection(_types); 
        }

        public CodeMemberCollection TypeMembers { get; private set; }
    }
}

