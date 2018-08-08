using MetaPrograms.CodeModel.Imperative.Expressions;

namespace MetaPrograms.CodeModel.Imperative.Members
{
    public class NamedPropertyValue
    {
        public NamedPropertyValue(IdentifierName name, AbstractExpression value)
        {
            Name = name;
            Value = value;
        }

        public IdentifierName Name { get; }
        public AbstractExpression Value { get; }
    }
}