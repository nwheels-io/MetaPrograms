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

            _member = new PropertyMember {
                Name = _symbol.Name,
                DeclaringType = _modelBuilder.TryGetMember<TypeMember>(_symbol.ContainingType),
                Status = MemberStatus.Incomplete,
                Visibility = _symbol.GetMemberVisibility(),
                Modifier = _symbol.GetMemberModifier(),
                PropertyType = _modelBuilder.TryGetMember<TypeMember>(_symbol.Type),
                Getter = _getterReader?.Member as MethodMember,
                Setter = _setterReader?.Member as MethodMember
            };
        }

        public void ReadAttributes()
        {
            _getterReader?.ReadAttributes();
            _setterReader?.ReadAttributes();
        }

        public void ReadImplementation()
        {
            _getterReader?.ReadImplementation();
            _setterReader?.ReadImplementation();
        }

        public ISymbol Symbol => _symbol;
        public AbstractMember Member => _member;
    }
}
