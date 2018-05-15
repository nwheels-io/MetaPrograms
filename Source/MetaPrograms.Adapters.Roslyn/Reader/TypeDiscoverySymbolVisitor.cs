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
        private readonly HashSet<INamedTypeSymbol> _doneSymbols = new HashSet<INamedTypeSymbol>();

        public TypeDiscoverySymbolVisitor(CodeModelBuilder modelBuilder, List<IPhasedTypeReader> results)
        {
            _modelBuilder = modelBuilder;
            _results = results;
        }

        public override void VisitNamedType(INamedTypeSymbol symbol)
        {
            if (!_doneSymbols.Add(symbol))
            {
                return;
            }

            RegisterTypeReader(symbol);
            base.VisitNamedType(symbol);
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
                    throw new NotImplementedException(symbol.TypeKind.ToString());
            }
        }
    }
}
