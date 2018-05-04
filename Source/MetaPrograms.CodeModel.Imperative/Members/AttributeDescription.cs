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

        public AttributeDescription(
            AttributeDescription source,
            Mutator<TypeMember>? attributeType,
            Mutator<ImmutableList<AbstractExpression>>? constructorArguments,
            Mutator<ImmutableList<PropertyValue>>? propertyValues)
        {
            AttributeType = attributeType.MutatedOrOriginal(source.AttributeType);
            ConstructorArguments = constructorArguments.MutatedOrOriginal(source.ConstructorArguments);
            PropertyValues = propertyValues.MutatedOrOriginal(source.PropertyValues);
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
        public PropertyValue(string name, AbstractExpression value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }
        public AbstractExpression Value { get; }
    }
}
