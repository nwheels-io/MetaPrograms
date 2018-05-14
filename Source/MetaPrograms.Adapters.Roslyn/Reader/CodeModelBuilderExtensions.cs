using System;
using MetaPrograms.CodeModel.Imperative;
using MetaPrograms.CodeModel.Imperative.Members;
using Microsoft.CodeAnalysis;

namespace MetaPrograms.Adapters.Roslyn.Reader
{
    public static class CodeModelBuilderExtensions
    {
        public static TypeMember IncludeType(this CodeModelBuilder modelBuilder, ITypeSymbol symbol, SemanticModel semanticModel)
        {
            return modelBuilder.GetOrAddMember(symbol, () => ReadType(modelBuilder, symbol, semanticModel));
        }

        public static TypeMember ReadType(this CodeModelBuilder modelBuilder, ITypeSymbol symbol, SemanticModel semanticModel)
        {
            switch (symbol.TypeKind)
            {
                case TypeKind.Class:
                    return new ClassReader(modelBuilder, semanticModel, (INamedTypeSymbol)symbol).Read();
                case TypeKind.TypeParameter:
                    return new TypeParameterReader(modelBuilder, semanticModel, (ITypeParameterSymbol)symbol).Read();
                default:
                    throw new NotImplementedException(symbol.TypeKind.ToString());
            }
        }
    }
}