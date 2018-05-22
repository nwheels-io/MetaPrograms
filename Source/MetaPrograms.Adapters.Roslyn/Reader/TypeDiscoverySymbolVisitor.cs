using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MetaPrograms.CodeModel.Imperative;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

namespace MetaPrograms.Adapters.Roslyn.Reader
{
    public class TypeDiscoverySymbolVisitor : SymbolVisitor
    {
        private readonly Compilation _compilation;
        private readonly CodeModelBuilder _modelBuilder;
        private readonly List<IPhasedTypeReader> _results;
        private readonly HashSet<INamedTypeSymbol> _includedSymbols = new HashSet<INamedTypeSymbol>();
        private int _codedTypeDescendLevel = 0;
        private int _anyTypeDescendLevel = 0;

        public TypeDiscoverySymbolVisitor(Compilation compilation, CodeModelBuilder modelBuilder, List<IPhasedTypeReader> results)
        {
            _compilation = compilation;
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
            if (symbol != null && ShouldIncludeType(symbol) && _includedSymbols.Add(symbol))
            {
                EnterType(symbol);

                try
                {
                    RegisterTypeReader(symbol);
                    VisitAttributes(symbol);

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

            if (symbol.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax() is MethodDeclarationSyntax methodSyntax)
            {
                var methodSemantic = _compilation.GetSemanticModel(methodSyntax.SyntaxTree, ignoreAccessibility: true);
                var bodyOperation = methodSemantic.GetOperation(methodSyntax.Body);
                var walker = new TypeDiscoveryOperationWalker(type => type.Accept(this));
                bodyOperation.Accept(walker);
            }
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
            if (//symbol.SpecialType == SpecialType.System_Object || 
                symbol.SpecialType == SpecialType.System_Void)
                //|| symbol.ToString() == "System.Type")
            {
                return false;
            }

            var referencedByIncludedType = (_codedTypeDescendLevel > 0);
            var externalFarFromCode = (_anyTypeDescendLevel - _codedTypeDescendLevel > 1);
            return (TypeHasSourceCode(symbol) || (referencedByIncludedType && !externalFarFromCode));
        }

        private void RegisterTypeReader(INamedTypeSymbol symbol)
        {
            var s = symbol.ToDisplayString();
            var readerMechanism = new TypeReaderMechanism(_modelBuilder, symbol);

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

        private static IEnumerable<ISymbol> QuerySymbolsLinkedToType(INamedTypeSymbol parent)
        {
            return new ISymbol[] { parent.BaseType }
                .Concat(parent.Interfaces)
                .Concat(parent.TypeArguments)
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
