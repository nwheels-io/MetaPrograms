using System;
using System.Collections.Generic;
using System.Text;
using MetaPrograms.CodeModel.Imperative.Members;

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
        TypeMember TypeMember { get; }
    }
}
