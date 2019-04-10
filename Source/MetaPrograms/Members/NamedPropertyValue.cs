using MetaPrograms.Expressions;

namespace MetaPrograms.Members
{
    public class NamedPropertyValue
    {
        public NamedPropertyValue(IdentifierName name, AbstractExpression value)
        {
            Name = name;
            Value = value;
        }

        public AbstractMember Member { get; }
        public IdentifierName Name { get; }
        public AbstractExpression Value { get; }
    }
}
