using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MetaPrograms.CodeModel.Imperative;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MetaPrograms.Adapters.Roslyn.Reader
{
    public class TypeDiscoverySymbolVisitor : SymbolVisitor
    {
        private readonly CodeModelBuilder _modelBuilder;
        private readonly List<IPhasedTypeReader> _results;
        private readonly HashSet<INamedTypeSymbol> _includedSymbols = new HashSet<INamedTypeSymbol>();
        private int _typeDescendLevel = 0;

        public TypeDiscoverySymbolVisitor(CodeModelBuilder modelBuilder, List<IPhasedTypeReader> results)
        {
            _modelBuilder = modelBuilder;
            _results = results;
        }

        public override void VisitNamespace(INamespaceSymbol symbol)
        {
            foreach (var memberSymbol in symbol.GetMembers())
            {
                memberSymbol.Accept(this);
            }
        }

        public override void VisitNamedType(INamedTypeSymbol symbol)
        {
            if (symbol != null && ShouldIncludeType(symbol) && _includedSymbols.Add(symbol))
            {
                EnterType();

                try
                {
                    RegisterTypeReader(symbol);

                    VisitNamedType(symbol.BaseType);

                    foreach (var interfaceSymbol in symbol.Interfaces)
                    {
                        VisitNamedType(interfaceSymbol);
                    }

                    foreach (var typeArgumentSymbol in symbol.TypeArguments)
                    {
                        typeArgumentSymbol.Accept(this);
                    }

                    foreach (var nestedTypeSymbol in symbol.GetTypeMembers())
                    {
                        nestedTypeSymbol.Accept(this);
                    }

                    base.VisitNamedType(symbol);
                }
                finally
                {
                    ExitType();
                }
            }
        }

        private bool ShouldIncludeType(INamedTypeSymbol symbol)
        {
            if (symbol.SpecialType == SpecialType.System_Object)
            {
                return false;
            }

            var hasSourceCode = (symbol.DeclaringSyntaxReferences.Length > 0);
            var referencedByIncludedType = (_typeDescendLevel > 0);

            return (hasSourceCode || referencedByIncludedType);
        }

        private void RegisterTypeReader(INamedTypeSymbol symbol)
        {
            var syntax = symbol.DeclaringSyntaxReferences.OfType<BaseTypeDeclarationSyntax>().FirstOrDefault();
            var readerMechanism = new TypeReaderMechanism(_modelBuilder, symbol);

            switch (symbol.TypeKind)
            {
                case TypeKind.Class:
                    _results.Add(new ClassReader(readerMechanism));
                    break;
                case TypeKind.Interface:
                    _results.Add(new InterfaceReader(readerMechanism));
                    break;
                default:
                    throw new NotImplementedException($"{symbol.TypeKind} '{symbol.Name}'");
            }
        }

        private void EnterType()
        {
            _typeDescendLevel++;
        }

        private void ExitType()
        {
            _typeDescendLevel--;
        }
    }
}
