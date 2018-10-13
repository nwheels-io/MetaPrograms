//using System.Linq;
//using System.Threading;
//using MetaPrograms;
//using Microsoft.CodeAnalysis;
//using Microsoft.CodeAnalysis.CSharp;
//using Microsoft.CodeAnalysis.CSharp.Syntax;

//namespace MetaPrograms.CSharp.Reader
//{
//    public class ProjectReader
//    {
//        private readonly CodeModelBuilder _modelBuilder;
//        private readonly Workspace _workspace;
//        private readonly Project _project;
//        private readonly Compilation _compilation;
//        private readonly SyntaxTree[] _syntaxTrees;

//        public ProjectReader(CodeModelBuilder modelBuilder, Workspace workspace, Project project)
//        {
//            _modelBuilder = modelBuilder;
//            _workspace = workspace;
//            _project = project;
//            _compilation = project.GetCompilationAsync(CancellationToken.None).Result;
//            _syntaxTrees = _compilation.SyntaxTrees.ToArray();
//        }

//        public void Read()
//        {
//            foreach (var tree in _syntaxTrees)
//            {
//                var semanticModel = _compilation.GetSemanticModel(tree);

//                var topLevelClasses = tree.GetCompilationUnitRoot()
//                    .DescendantNodes(descendIntoChildren: MayContainTopLevelClasses)
//                    .OfType<ClassDeclarationSyntax>()
//                    .ToList();

//                topLevelClasses.ForEach(classSyntax => ReadTopLevelClass(classSyntax));
//            }

//            bool MayContainTopLevelClasses(SyntaxNode node)
//            {
//                return (node is CompilationUnitSyntax || node is NamespaceDeclarationSyntax);
//            }
//        }

//        private void ReadTopLevelClass(ClassDeclarationSyntax classSyntax)
//        {
//            var classReader = new ClassReader(_modelBuilder, classSyntax);
//            classReader.Read();
//        }
//    }
//}