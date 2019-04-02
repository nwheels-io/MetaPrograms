using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MetaPrograms;
using MetaPrograms.Members;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

namespace MetaPrograms.CSharp.Reader
{
    public class TypeDiscoverySymbolVisitor : SymbolVisitor
    {
        private readonly CodeModelBuilder _modelBuilder;
        private readonly List<IPhasedTypeReader> _results;
        private readonly HashSet<INamedTypeSymbol> _includedSymbols = new HashSet<INamedTypeSymbol>();
        private readonly HashSet<string> _includedTypeMetadataNames = new HashSet<string>();
        private int _codedTypeDescendLevel = 0;
        private int _anyTypeDescendLevel = 0;

        public TypeDiscoverySymbolVisitor(CodeModelBuilder modelBuilder, List<IPhasedTypeReader> results)
        {
            _modelBuilder = modelBuilder;
            _results = results;
        }

        public void VisitAttributes(ISymbol symbol)
        {
            VisitAttributes(symbol.GetAttributes());
        }

        public void VisitAttributes(IEnumerable<AttributeData> attributes)
        {
            foreach (var attribute in attributes)
            {
                attribute.AttributeClass.Accept(this);
            }
        }

        public override void VisitNamespace(INamespaceSymbol symbol)
        {
            VisitAttributes(symbol);

            foreach (var memberSymbol in symbol.GetMembers())
            {
                memberSymbol.Accept(this);
            }
        }

        public override void VisitNamedType(INamedTypeSymbol symbol)
        {
            IncludeType(symbol);
        }

        private void IncludeType(INamedTypeSymbol symbol, bool force = false)
        {            
            if (symbol != null && (force || ShouldIncludeType(symbol)) && _includedSymbols.Add(symbol))
            {
                var systemTypeMetadataName = symbol.GetSystemTypeMetadataName();
                if (!_includedTypeMetadataNames.Add(systemTypeMetadataName))
                {
                    return;
                }

                EnterType(symbol);

                try
                {
                    RegisterTypeReader(symbol, systemTypeMetadataName);
                    VisitAttributes(symbol);
                    VisitGenericArguments(symbol);
                    IncludeType(symbol.BaseType, force: true);
                    IncludeType(symbol.ContainingType, force: true);

                    var allLinkedSymbols = QuerySymbolsLinkedToType(symbol);

                    foreach (var linkedSymbol in allLinkedSymbols)
                    {
                        linkedSymbol.Accept(this);
                    }
                }
                finally
                {
                    ExitType(symbol);
                }
            }
        }

        private void VisitGenericArguments(INamedTypeSymbol symbol)
        {
            foreach (var argument in symbol.TypeArguments.OfType<INamedTypeSymbol>())
            {
                IncludeType(argument, force: true);
            }
        }

        public override void VisitArrayType(IArrayTypeSymbol symbol)
        {
            symbol.ElementType.Accept(this);
        }

        public override void VisitField(IFieldSymbol symbol)
        {
            VisitAttributes(symbol);

            if (symbol.Type is INamedTypeSymbol type)
            {
                VisitNamedType(type);
            }
        }

        public override void VisitMethod(IMethodSymbol symbol)
        {
            VisitAttributes(symbol);
            VisitAttributes(symbol.GetReturnTypeAttributes());

            var allLinkedSymbols = QuerySymbolsLinkedToMethod(symbol);

            foreach (var linkedSymbol in allLinkedSymbols)
            {
                linkedSymbol.Accept(this);
            }

            if (!symbol.IsAbstract)
            {
                var syntaxRef = symbol.DeclaringSyntaxReferences.FirstOrDefault();
                if (syntaxRef != null)
                {
                    VisitMethodBody(syntaxRef.GetSyntax());
                }
            }
        }

        public override void VisitProperty(IPropertySymbol symbol)
        {
            symbol.Type.Accept(this);
            VisitAttributes(symbol);

            symbol.GetMethod?.Accept(this);
            symbol.SetMethod?.Accept(this);
        }

        public override void VisitEvent(IEventSymbol symbol)
        {
            VisitAttributes(symbol);
            symbol.Type.Accept(this);
        }

        public override void VisitParameter(IParameterSymbol symbol)
        {
            VisitAttributes(symbol);
            symbol.Type.Accept(this);
        }

        public override void VisitLocal(ILocalSymbol symbol)
        {
            VisitAttributes(symbol);
            symbol.Type.Accept(this);
        }

        private bool ShouldIncludeType(INamedTypeSymbol symbol)
        {
            if (symbol.SpecialType == SpecialType.System_Void)
            {
                return false;
            }

            var referencedByIncludedType = (_codedTypeDescendLevel > 0);
            var externalFarFromCode = (_anyTypeDescendLevel - _codedTypeDescendLevel > 1);
            return (TypeHasSourceCode(symbol) || (referencedByIncludedType && !externalFarFromCode));
        }

        private void RegisterTypeReader(INamedTypeSymbol symbol, string systemTypeMetadataName)
        {
            var readerMechanism = new TypeReaderMechanism(_modelBuilder, symbol, systemTypeMetadataName);

            switch (symbol.TypeKind)
            {
                case TypeKind.Class:
                case TypeKind.Delegate:
                    _results.Add(new ClassReader(readerMechanism));
                    break;
                case TypeKind.Interface:
                    _results.Add(new InterfaceReader(readerMechanism));
                    break;
                case TypeKind.Struct:
                    _results.Add(new StructReader(readerMechanism));
                    break;
                case TypeKind.Enum:
                    _results.Add(new EnumReader(readerMechanism));
                    break;
                default:
                    throw new NotImplementedException($"{symbol.TypeKind} '{symbol.Name}'");
            }
        }

        private void EnterType(INamedTypeSymbol type)
        {
            _anyTypeDescendLevel++;

            if (TypeHasSourceCode(type))
            {
                _codedTypeDescendLevel++;
            }
        }

        private void ExitType(INamedTypeSymbol type)
        {
            _anyTypeDescendLevel--;

            if (TypeHasSourceCode(type))
            {
                _codedTypeDescendLevel--;
            }
        }

        private void VisitMethodBody(SyntaxNode memberSyntax)
        {
            var compilation = _modelBuilder.GetCompilation(memberSyntax.SyntaxTree);
            var methodSemantic = compilation.GetSemanticModel(memberSyntax.SyntaxTree, ignoreAccessibility: true);

            var bodySyntax = (memberSyntax is MethodDeclarationSyntax method ? method.Body : memberSyntax);            
            var bodyOperation = methodSemantic.GetOperation(bodySyntax);

            if (bodyOperation != null)
            {
                var walker = new TypeDiscoveryOperationWalker(type => type.Accept(this));
                bodyOperation.Accept(walker);
            }
        }

        private static IEnumerable<ISymbol> QuerySymbolsLinkedToType(INamedTypeSymbol parent)
        {
            return parent.Interfaces
                .Concat(parent.GetMembers())
                .Where(s => s != null);
        }

        private static IEnumerable<ISymbol> QuerySymbolsLinkedToMethod(IMethodSymbol parent)
        {
            return new ISymbol[] { parent.ReturnType }
                .Concat(parent.Parameters)
                .Where(s => s != null);
        }

        private static bool TypeHasSourceCode(INamedTypeSymbol type) => (type.DeclaringSyntaxReferences.Length > 0);
    }
}
