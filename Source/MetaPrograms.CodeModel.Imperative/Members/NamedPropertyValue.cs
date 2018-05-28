using MetaPrograms.CodeModel.Imperative.Expressions;

namespace MetaPrograms.CodeModel.Imperative.Members
{
    public class NamedPropertyValue
    {
        public NamedPropertyValue(string name, AbstractExpression value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }
        public AbstractExpression Value { get; }
    }
}