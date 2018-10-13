using System;
using System.Collections.Generic;
using System.Text;
using MetaPrograms;
using MetaPrograms.Members;
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
