using System;
using System.Collections.Generic;
using System.Text;
using MetaPrograms.Members;
using Microsoft.CodeAnalysis;

namespace MetaPrograms.Adapters.Roslyn.Reader
{
    public interface IPhasedMemberReader
    {
        void ReadDeclaration();
        void ReadAttributes();
        void ReadImplementation();
        ISymbol Symbol { get; }
        AbstractMember Member { get; }
    }
}
