using System;
using System.Collections.Generic;
using System.Text;
using MetaPrograms.CodeModel.Imperative;
using MetaPrograms.CodeModel.Imperative.Members;
using Microsoft.CodeAnalysis;

namespace MetaPrograms.Adapters.Roslyn.Reader
{
    public static class TypeParameterReader
    {
        public static TypeMember Read(ITypeParameterSymbol symbol)
        {
            return new TypeMember {
                Name = symbol.Name,
                IsGenericParameter = true
            };
        }
    }
}
