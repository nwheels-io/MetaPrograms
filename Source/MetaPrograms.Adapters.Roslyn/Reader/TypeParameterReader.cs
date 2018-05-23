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
            var memberBuilder  = new TypeMemberBuilder();

            memberBuilder.Name = symbol.Name;
            memberBuilder.IsGenericParameter = true;

            return new RealTypeMember(memberBuilder);
        }
    }
}
