using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using MetaPrograms;
using MetaPrograms.Members;
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
            _member = new FieldMember {
                Name = _symbol.Name,
                DeclaringType = _modelBuilder.TryGetMember<TypeMember>(_symbol.ContainingType),
                Status = MemberStatus.Incomplete,
                Visibility = _symbol.GetMemberVisibility(),
                Modifier = _symbol.GetMemberModifier(),
                Type = _modelBuilder.TryGetMember<TypeMember>(_symbol.Type),
                IsReadOnly = _symbol.IsReadOnly,
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
