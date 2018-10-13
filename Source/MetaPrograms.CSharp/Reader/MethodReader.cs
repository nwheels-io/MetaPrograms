using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using MetaPrograms;
using MetaPrograms.Members;
using Microsoft.CodeAnalysis;

namespace MetaPrograms.CSharp.Reader
{
    public class MethodReader : IPhasedMemberReader
    {
        private readonly CodeModelBuilder _modelBuilder;
        private readonly IMethodSymbol _symbol;
        private MethodMember _member;
        
        public MethodReader(CodeModelBuilder modelBuilder, IMethodSymbol symbol)
        {
            _modelBuilder = modelBuilder;
            _symbol = symbol;
            _member = null;
        }

        public void ReadDeclaration()
        {
            _member = new MethodMember {
                Name = _symbol.Name,
                DeclaringType = _modelBuilder.TryGetMember<TypeMember>(_symbol.ContainingType),
                Status = MemberStatus.Incomplete,
                Visibility = _symbol.GetMemberVisibility(),
                Modifier = _symbol.GetMemberModifier(),
                Signature = MethodReaderMechanism.ReadSignature(_modelBuilder, _symbol),
            };
        }

        public void ReadAttributes()
        {
            Member.Attributes.AddRange(AttributeReader.ReadSymbolAttributes(_modelBuilder, Symbol));
        }

        public void ReadImplementation()
        {
            MethodReaderMechanism.ReadBody(_modelBuilder, _symbol, _member);
        }

        public ISymbol Symbol => _symbol;
        public AbstractMember Member => _member;
    }
}
