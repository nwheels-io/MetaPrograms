using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using MetaPrograms.CodeModel.Imperative.Expressions;

namespace MetaPrograms.CodeModel.Imperative.Members
{
    public class AttributeDescription
    {
        public override string ToString()
        {
            return (AttributeType?.Name?.ToString() ?? base.ToString());
        }

        public TypeMember AttributeType { get; set; }
        public List<AbstractExpression> ConstructorArguments { get; set; } = new List<AbstractExpression>();
        public List<NamedPropertyValue> PropertyValues { get; set; } = new List<NamedPropertyValue>();
    }
}
