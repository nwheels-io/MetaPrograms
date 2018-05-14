using System;
using System.Collections.Generic;
using System.Text;
using MetaPrograms.CodeModel.Imperative;
using MetaPrograms.CodeModel.Imperative.Members;
using Microsoft.CodeAnalysis;

namespace MetaPrograms.Adapters.Roslyn.Reader
{
    public class TypeParameterReader
    {
        private readonly CodeModelBuilder _modelBuilder;
        private readonly SemanticModel _semanticModel;
        private readonly ITypeParameterSymbol _symbol;

        public TypeParameterReader(CodeModelBuilder modelBuilder, SemanticModel semanticModel, ITypeParameterSymbol symbol)
        {
            _modelBuilder = modelBuilder;
            _semanticModel = semanticModel;
            _symbol = symbol;
        }

        public TypeMember Read()
        {
            var memberBuilder  = new TypeMemberBuilder();

            memberBuilder.Name = _symbol.Name;
            memberBuilder.IsGenericParameter = true;

            return new TypeMember(memberBuilder);
        }
    }
}
