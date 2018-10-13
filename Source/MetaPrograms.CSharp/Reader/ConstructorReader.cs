using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using MetaPrograms;
using MetaPrograms.Members;
using Microsoft.CodeAnalysis;

namespace MetaPrograms.CSharp.Reader
{
    public class ConstructorReader : IPhasedMemberReader
    {
        private readonly CodeModelBuilder _modelBuilder;
        private readonly IMethodSymbol _symbol;
        private ConstructorMember _member;

        public ConstructorReader(CodeModelBuilder modelBuilder, IMethodSymbol symbol)
        {
            _modelBuilder = modelBuilder;
            _symbol = symbol;
            _member = null;
        }

        public void ReadDeclaration()
        {
            _member = new ConstructorMember {
                DeclaringType = _modelBuilder.TryGetMember<TypeMember>(_symbol.ContainingType),
                Status = MemberStatus.Incomplete,
                Visibility = _symbol.GetMemberVisibility(),
                Modifier = _symbol.GetMemberModifier(),
                Signature = MethodReaderMechanism.ReadSignature(_modelBuilder, _symbol),
            };        
        }

        public void ReadAttributes()
        {
            //TODO: implement this
            //throw new NotImplementedException();
        }

        public void ReadImplementation()
        {
            MethodReaderMechanism.ReadBody(_modelBuilder, _symbol, _member);
        }

        public ISymbol Symbol => _symbol;
        public AbstractMember Member => _member;
    }
}
