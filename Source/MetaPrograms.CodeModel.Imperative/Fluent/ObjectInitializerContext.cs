using System;
using System.Collections.Generic;
using System.Text;
using MetaPrograms.CodeModel.Imperative.Expressions;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Fluent
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
