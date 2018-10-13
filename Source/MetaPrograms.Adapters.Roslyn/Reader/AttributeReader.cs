using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using MetaPrograms;
using MetaPrograms.Expressions;
using MetaPrograms.Members;
using Microsoft.CodeAnalysis;

namespace MetaPrograms.Adapters.Roslyn.Reader
{
    public static class AttributeReader
    {
        public static AttributeDescription ReadAttribute(CodeModelBuilder modelBuilder, AttributeData data)
        {
            var constructorArguments = data.ConstructorArguments
                .Select(arg => ExpressionReader.ReadTypedConstant(modelBuilder, arg))
                .ToList<AbstractExpression>();

            var propertyValues = data.NamedArguments
                .Select(kvp => new NamedPropertyValue(kvp.Key, ExpressionReader.ReadTypedConstant(modelBuilder, kvp.Value)))
                .ToList();

            var result = new AttributeDescription {
                AttributeType = modelBuilder.TryGetMember<TypeMember>(data.AttributeClass),
                ConstructorArguments = constructorArguments,
                PropertyValues = propertyValues
            };

            return result;
        }

        public static IEnumerable<AttributeDescription> ReadSymbolAttributes(CodeModelBuilder modelBuilder, ISymbol symbol)
        {
            return symbol
                .GetAttributes()
                .Select(attr => AttributeReader.ReadAttribute(modelBuilder, attr));
        }
    }
}
