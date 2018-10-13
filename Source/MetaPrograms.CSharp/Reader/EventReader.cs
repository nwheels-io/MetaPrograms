using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using MetaPrograms;
using MetaPrograms.Members;
using Microsoft.CodeAnalysis;

namespace MetaPrograms.CSharp.Reader
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

            _member = new EventMember {
                Name = _symbol.Name,
                DeclaringType = _modelBuilder.TryGetMember<TypeMember>(_symbol.ContainingType),
                Status = MemberStatus.Incomplete,
                Visibility = _symbol.GetMemberVisibility(),
                Modifier = _symbol.GetMemberModifier(),
                DelegateType = _modelBuilder.TryGetMember<TypeMember>(_symbol.Type),
            };
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
