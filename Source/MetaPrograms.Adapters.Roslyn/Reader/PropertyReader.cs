using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using MetaPrograms.CodeModel.Imperative;
using MetaPrograms.CodeModel.Imperative.Members;
using Microsoft.CodeAnalysis;

namespace MetaPrograms.Adapters.Roslyn.Reader
{
    public class PropertyReader : IPhasedMemberReader
    {
        private readonly CodeModelBuilder _modelBuilder;
        private readonly IPropertySymbol _symbol;
        private PropertyMember _member;
        private MethodReader _getterReader;
        private MethodReader _setterReader;

        public PropertyReader(CodeModelBuilder modelBuilder, IPropertySymbol symbol)
        {
            _modelBuilder = modelBuilder;
            _symbol = symbol;
            _member = null;
        }

        public void ReadDeclaration()
        {
            _getterReader = MethodReaderMechanism.CreateAccessorMethodReader(_modelBuilder, _symbol.GetMethod);
            _setterReader = MethodReaderMechanism.CreateAccessorMethodReader(_modelBuilder, _symbol.SetMethod);

            _member = new PropertyMember(
                name: _symbol.Name,
                declaringType: _modelBuilder.GetMember<TypeMember>(_symbol.ContainingType),
                status: MemberStatus.Incomplete,
                visibility: _symbol.GetMemberVisibility(),
                modifier: _symbol.GetMemberModifier(),
                attributes: ImmutableList<AttributeDescription>.Empty,
                propertyType: _modelBuilder.GetMember<TypeMember>(_symbol.Type),
                getter: MethodReader.GetMemberRef(_getterReader),
                setter: MethodReader.GetMemberRef(_setterReader));
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
