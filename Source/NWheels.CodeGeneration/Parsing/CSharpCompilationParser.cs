using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using NWheels.CodeGeneration.CodeModel.Members;

namespace NWheels.CodeGeneration.Parsing
{
    public class CSharpCompilationParser
    {
        private readonly List<SyntaxTree> _syntaxTrees = new List<SyntaxTree>();
        private readonly List<TypeMember> _types = new List<TypeMember>();
        private readonly Dictionary<string, MetadataReference> _referencedAssemblies = new Dictionary<string, MetadataReference>();
        private CSharpCompilation _compilation;

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public void AddSystemReferences(params string[] assemblyNames)
        {
            var systemAssemblyPaths = GetSystemAssemblyPaths(assemblyNames);

            foreach (var path in systemAssemblyPaths)
            {
                AddReference(path);
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public void AddReference(string assemblyFilePath)
        {
            if (!_referencedAssemblies.ContainsKey(assemblyFilePath))
            {
                _referencedAssemblies.Add(assemblyFilePath, MetadataReference.CreateFromFile(assemblyFilePath));
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public void Parse(Stream input)
        {
            var source = SourceText.From(input);
            var syntaxTree = CSharpSyntaxTree.ParseText(source);

            _syntaxTrees.Add(syntaxTree);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public void Analyze()
        {
            AddSystemReferences();

            _compilation = CSharpCompilation.Create(
                "ProgrammingModel",
                syntaxTrees: _syntaxTrees, 
                references: _referencedAssemblies.Values, 
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            CheckForSourceErrors();

            foreach (var tree in _syntaxTrees)
            {
                AnalyzeSyntaxTree(tree);
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public IEnumerable<TypeMember> Types => _types;

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private void CheckForSourceErrors()
        {
            bool IsError(Diagnostic diagnostic) => 
                diagnostic.Severity == DiagnosticSeverity.Error || 
                diagnostic.IsWarningAsError;

            var diagnostics = _compilation.GetDiagnostics();

            if (diagnostics.Any(IsError))
            {
                throw new SourceCodeErrorsException("Source code has errors", diagnostics.Where(IsError));
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private void AnalyzeSyntaxTree(SyntaxTree tree)
        {
            var semanticModel = _compilation.GetSemanticModel(tree);

            var topLevelClasses = tree.GetCompilationUnitRoot()
                .DescendantNodes(descendIntoChildren: node => {
                    var result = (node is CompilationUnitSyntax ||  node is NamespaceDeclarationSyntax);
                    return result;
                })
                .OfType<ClassDeclarationSyntax>()
                .ToList();

            foreach (var singleClass in topLevelClasses)
            {
                _types.Add(ParseClass(semanticModel, singleClass));
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private TypeMember ParseClass(SemanticModel model, ClassDeclarationSyntax syntax)
        {
            var symbol = model.GetDeclaredSymbol(syntax);
            var member = new TypeMember(
                namespaceName: symbol.ContainingNamespace.Name,
                name: symbol.Name,
                visibility: MemberVisibility.Public,
                typeKind: TypeMemberKind.Class);

            return member;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private static readonly IReadOnlyList<string> _s_trustedAssemblyPaths = 
            AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES").ToString().Split(';');

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private static string[] GetSystemAssemblyPaths(string[] assemblyNames)
        {
            var assemblyNameSet = new HashSet<string>(assemblyNames.Select(name => name + ".dll"));

            assemblyNameSet.Add("mscorlib.dll");
            assemblyNameSet.Add("System.Runtime.dll");
            assemblyNameSet.Add("System.Private.CoreLib.dll");
            assemblyNameSet.Add("System.Private.Library.dll");

            var selectedAssemblyPaths = _s_trustedAssemblyPaths
                .Where(path => assemblyNameSet.Contains(Path.GetFileName(path)))
                .ToArray();

            return selectedAssemblyPaths;
        }
    }
}
