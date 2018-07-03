using System;
using System.Collections.Generic;
using System.Text;
using MetaPrograms.CodeModel.Imperative.Expressions;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Fluent
{
    public class ObjectInitializerContext
    {
        private readonly List<NamedPropertyValue> _propertyValues = new List<NamedPropertyValue>();

        public void Add(string name, AbstractExpression value)
        {
            _propertyValues.Add(new NamedPropertyValue(name, value));
        }

        public IReadOnlyList<NamedPropertyValue> PropertyValues => _propertyValues;
    }
}
