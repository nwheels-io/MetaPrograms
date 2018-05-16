using System;
using System.Collections.Generic;
using System.Text;
using MetaPrograms.CodeModel.Imperative.Members;
using Microsoft.CodeAnalysis;

namespace MetaPrograms.Adapters.Roslyn.Reader
{
    public class FieldReader : IPhasedMemberReader
    {
        public FieldReader(IFieldSymbol symbol)
        {
            Symbol = symbol;
        }

        public void ReadDeclaration()
        {
            //throw new NotImplementedException();
        }

        public void ReadAttributes()
        {
            //throw new NotImplementedException();
        }

        public void ReadImplementation()
        {
            //throw new NotImplementedException();
        }

        public ISymbol Symbol { get; }
        public AbstractMember Member { get; }
    }
}
