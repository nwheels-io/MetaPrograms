using System;
using System.Collections.Generic;
using System.Text;
using MetaPrograms.Expressions;
using MetaPrograms.Members;

namespace MetaPrograms.Fluent
{
    public class ObjectInitializerContext
    {
        public void Add(string name, AbstractExpression value)
        {
            PropertyValues.Add(new NamedPropertyValue(name, value));
        }

        public List<NamedPropertyValue> PropertyValues { get; } = new List<NamedPropertyValue>();
    }
}
