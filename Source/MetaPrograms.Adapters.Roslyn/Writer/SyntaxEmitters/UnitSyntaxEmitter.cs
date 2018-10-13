using System;
using System.Collections.Generic;
using System.Linq;
using MetaPrograms;
using MetaPrograms.Members;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MetaPrograms.Adapters.Roslyn.Writer.SyntaxEmitters
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
                var unitSyntax = SyntaxFactory.CompilationUnit()
                    .WithUsings(SyntaxFactory.List(usingSyntaxList))
                    .WithMembers(
                        SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
                            SyntaxFactory.NamespaceDeclaration(CreateQualifiedNameSyntax(_type.Namespace.Split('.')))
                                .WithMembers(SyntaxFactory.SingletonList(typeSyntax))))
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
                    return SyntaxFactory.IdentifierName(parts[0]);
                default:
                    return SyntaxFactory.QualifiedName(
                        CreateQualifiedNameSyntax(parts.Slice(0, parts.Length - 1)), 
                        SyntaxFactory.IdentifierName(parts[parts.Length - 1])
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
            sortedUsings = sortedNamespacesToImport.Select(ns => SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(ns))).ToArray();
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
