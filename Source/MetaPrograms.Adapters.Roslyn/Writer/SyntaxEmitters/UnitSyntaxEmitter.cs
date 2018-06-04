using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MetaPrograms.Adapters.Roslyn.Writer.SyntaxEmitters;
using MetaPrograms.CodeModel.Imperative;
using MetaPrograms.CodeModel.Imperative.Fluent;
using MetaPrograms.CodeModel.Imperative.Members;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MetaPrograms.Adapters.Roslyn.Writer
{
    public class UnitSyntaxEmitter
    {
        private readonly ImperativeCodeModel _codeModel;
        private readonly TypeMember _type;

        public UnitSyntaxEmitter(ImperativeCodeModel codeModel, TypeMember type)
        {
            _codeModel = codeModel;
            _type = type;
        }

        public SyntaxTree EmitSyntax()
        {
            DetermineNamespaceImports(GetReferencedTypes(), out var usingSyntaxList, out var importContext);

            using (CodeGeneratorContext.GetContextOrThrow().PushState(importContext))
            {
                var typeSyntax = TypeSyntaxEmitter.GetSyntax(_type);
                var unitSyntax = CompilationUnit()
                    .WithUsings(List(usingSyntaxList))
                    .WithMembers(
                        SingletonList<MemberDeclarationSyntax>(
                            NamespaceDeclaration(CreateQualifiedNameSyntax(_type.Namespace.Split('.')))
                                .WithMembers(SingletonList(typeSyntax))))
                    .NormalizeWhitespace();

                return unitSyntax.SyntaxTree;
            }
        }

        private NameSyntax CreateQualifiedNameSyntax(Span<string> parts)
        {
            switch (parts.Length)
            {
                case 0:
                    throw new ArgumentException("At least one name part must exist", nameof(parts));
                case 1:
                    return IdentifierName(parts[0]);
                default:
                    return QualifiedName(
                        CreateQualifiedNameSyntax(parts.Slice(0, parts.Length - 1)), 
                        IdentifierName(parts[parts.Length - 1])
                    );
            }
        }

        private void DetermineNamespaceImports(
            IReadOnlyCollection<TypeMember> referencedTypes, 
            out UsingDirectiveSyntax[] sortedUsings, 
            out ImportContext importContext)
        {
            importContext = new ImportContext();
            var typesGroupedByName = referencedTypes.GroupBy(t => t.MakeNameWithGenericArity('#'));

            foreach (var identicalNameGroup in typesGroupedByName)
            {
                var identicalNameTypes = identicalNameGroup.Take(2).ToArray();
                if (identicalNameTypes.Length == 1 && !string.IsNullOrEmpty(identicalNameTypes[0].Namespace))
                {
                    importContext.ImportType(identicalNameTypes[0]);
                }
            }

            var sortedNamespacesToImport = new List<string>(importContext.ImportedNamespaces.Where(ns => ns != _type.Namespace));
            sortedNamespacesToImport.Sort(new NamespaceImportComparer());
            sortedUsings = sortedNamespacesToImport.Select(ns => UsingDirective(ParseName(ns))).ToArray();
        }

        private IReadOnlyCollection<TypeMember> GetReferencedTypes()
        {
            var referencedTypes = new HashSet<TypeMember>();
            var visitor = new TypeReferenceMemberVisitor(referencedTypes);

            _type.AcceptVisitor(visitor);

            return referencedTypes;
        }

        private class NamespaceImportComparer : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                var xIsSystem = (x == "System" || x.StartsWith("System."));
                var yIsSystem = (y == "System" || y.StartsWith("System."));

                if (xIsSystem && !yIsSystem)
                {
                    return -1;
                }

                if (yIsSystem && !xIsSystem)
                {
                    return 1;
                }

                return x.CompareTo(y);
            }
        }
    }
}
