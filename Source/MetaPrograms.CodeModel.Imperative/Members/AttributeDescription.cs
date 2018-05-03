using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using MetaPrograms.CodeModel.Imperative.Expressions;

namespace MetaPrograms.CodeModel.Imperative.Members
{
    public class AttributeDescription
    {
        public AttributeDescription(
            TypeMember attributeType, 
            ImmutableList<AbstractExpression> constructorArguments, 
            ImmutableList<PropertyValue> propertyValues)
        {
            AttributeType = attributeType;
            ConstructorArguments = constructorArguments;
            PropertyValues = propertyValues;
        }

        public override string ToString()
        {
            return (AttributeType?.Name ?? base.ToString());
        }

        public TypeMember AttributeType { get; }
        public ImmutableList<AbstractExpression> ConstructorArguments { get; }
        public ImmutableList<PropertyValue> PropertyValues { get; }
    }

    public class PropertyValue
    {
        public string Name { get; }
        public AbstractExpression Value { get; }
    }
}
