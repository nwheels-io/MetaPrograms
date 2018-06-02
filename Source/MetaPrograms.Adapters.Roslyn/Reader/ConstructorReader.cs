using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using MetaPrograms.CodeModel.Imperative;
using MetaPrograms.CodeModel.Imperative.Members;
using Microsoft.CodeAnalysis;

namespace MetaPrograms.Adapters.Roslyn.Reader
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
            _member = new ConstructorMember(
                declaringType: _modelBuilder.TryGetMember<TypeMember>(_symbol.ContainingType),
                status: MemberStatus.Incomplete,
                visibility: _symbol.GetMemberVisibility(),
                modifier: _symbol.GetMemberModifier(),
                attributes: ImmutableList<AttributeDescription>.Empty,
                signature: MethodReaderMechanism.ReadSignature(_modelBuilder, _symbol),
                body: null,
                callBaseConstructor: null,
                callThisConstructor: null);        
        }

        public void ReadAttributes()
        {
            throw new NotImplementedException();
        }

        public void ReadImplementation()
        {
            throw new NotImplementedException();
        }

        public ISymbol Symbol => _symbol;
        public AbstractMember Member => _member;
    }
}
