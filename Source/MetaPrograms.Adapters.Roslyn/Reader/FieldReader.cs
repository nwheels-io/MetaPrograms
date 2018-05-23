using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using MetaPrograms.CodeModel.Imperative;
using MetaPrograms.CodeModel.Imperative.Members;
using Microsoft.CodeAnalysis;

namespace MetaPrograms.Adapters.Roslyn.Reader
{
    public class FieldReader : IPhasedMemberReader
    {
        private readonly CodeModelBuilder _modelBuilder;
        private readonly IFieldSymbol _symbol;
        private FieldMember _member;

        public FieldReader(CodeModelBuilder modelBuilder, IFieldSymbol symbol)
        {
            _modelBuilder = modelBuilder;
            _symbol = symbol;
            _member = null;
        }

        public void ReadDeclaration()
        {
            _member = new FieldMember(
                name: _symbol.Name,
                declaringType: _modelBuilder.TryGetMember<TypeMember>(_symbol.ContainingType),
                status: MemberStatus.Incomplete,
                visibility: _symbol.GetMemberVisibility(),
                modifier: _symbol.GetMemberModifier(),
                attributes: ImmutableList<AttributeDescription>.Empty,
                type: _modelBuilder.TryGetMember<TypeMember>(_symbol.Type),
                isReadOnly: _symbol.IsReadOnly,
                initializer: null);
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
