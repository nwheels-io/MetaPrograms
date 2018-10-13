using System;
using System.Collections.Generic;
using System.Text;
using MetaPrograms.Members;
using Microsoft.CodeAnalysis;

namespace MetaPrograms.Adapters.Roslyn.Reader
{
    public interface IPhasedTypeReader
    {
        void ReadName();
        void RegisterProxy();
        void ReadGenerics();
        void ReadAncestors();
        void ReadMemberDeclarations();
        void ReadAttributes();
        void ReadMemberImplementations();
        void RegisterReal();
        INamedTypeSymbol TypeSymbol { get; }
        TypeMember TypeMember { get; }
    }
}
