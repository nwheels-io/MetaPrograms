using System;
using System.Collections.Generic;
using System.Text;

namespace MetaPrograms.Adapters.Roslyn.Reader
{
    public interface IPhasedTypeReader
    {
        void ReadHeader();
        void ReadMemberDeclarations();
        void ReadAttributes();
        void ReadMemberImplementations();
    }
}
