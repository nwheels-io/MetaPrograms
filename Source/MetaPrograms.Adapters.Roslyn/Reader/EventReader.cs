using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using MetaPrograms.CodeModel.Imperative;
using MetaPrograms.CodeModel.Imperative.Members;
using Microsoft.CodeAnalysis;

namespace MetaPrograms.Adapters.Roslyn.Reader
{
    public class EventReader : IPhasedMemberReader
    {
        private readonly CodeModelBuilder _modelBuilder;
        private readonly IEventSymbol _symbol;
        private EventMember _member;
        private MethodReader _adderReader;
        private MethodReader _removerReader;

        public EventReader(CodeModelBuilder modelBuilder, IEventSymbol symbol)
        {
            _modelBuilder = modelBuilder;
            _symbol = symbol;
            _member = null;
        }

        public void ReadDeclaration()
        {
            _adderReader = MethodReaderMechanism.CreateAccessorMethodReader(_modelBuilder, _symbol.AddMethod);
            _removerReader = MethodReaderMechanism.CreateAccessorMethodReader(_modelBuilder, _symbol.RemoveMethod);

            _member = new EventMember(
                name: _symbol.Name,
                declaringType: _modelBuilder.TryGetMember<TypeMember>(_symbol.ContainingType),
                status: MemberStatus.Incomplete,
                visibility: _symbol.GetMemberVisibility(),
                modifier: _symbol.GetMemberModifier(),
                attributes: ImmutableList<AttributeDescription>.Empty,
                delegateType: _modelBuilder.TryGetMember<TypeMember>(_symbol.Type),
                adder: MethodReader.GetMemberRef(_adderReader),
                remover: MethodReader.GetMemberRef(_removerReader));
        }

        public void ReadAttributes()
        {
            //throw new NotImplementedException();
        }

        public void ReadImplementation()
        {
            //throw new NotImplementedException();
        }

        public ISymbol Symbol => _symbol;
        public AbstractMember Member => _member;
    }
}
