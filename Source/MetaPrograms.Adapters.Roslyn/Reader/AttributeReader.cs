using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using MetaPrograms.CodeModel.Imperative;
using MetaPrograms.CodeModel.Imperative.Expressions;
using MetaPrograms.CodeModel.Imperative.Members;
using Microsoft.CodeAnalysis;

namespace MetaPrograms.Adapters.Roslyn.Reader
{
    public static class AttributeReader
    {
        public static AttributeDescription ReadAttribute(CodeModelBuilder modelBuilder, AttributeData data)
        {
            var constructorArguments = data.ConstructorArguments
                .Select(arg => ExpressionReader.ReadTypedConstant(modelBuilder, arg))
                .ToImmutableList<AbstractExpression>();

            var propertyValues = data.NamedArguments
                .Select(kvp => new PropertyValue(kvp.Key, ExpressionReader.ReadTypedConstant(modelBuilder, kvp.Value)))
                .ToImmutableList();

            return new AttributeDescription(
                modelBuilder.TryGetMember<TypeMember>(data.AttributeClass),
                constructorArguments,
                propertyValues);
        }
    }
}
