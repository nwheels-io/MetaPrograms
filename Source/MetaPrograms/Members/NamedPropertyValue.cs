using MetaPrograms.Expressions;

namespace MetaPrograms.Members
{
    public class NamedPropertyValue
    {
        public NamedPropertyValue()
        {
        }

        public NamedPropertyValue(IdentifierName name, AbstractExpression value)
        {
            Name = name;
            Value = value;
        }

        public AbstractMember Member { get; set; }
        public IdentifierName Name { get; set; }
        public AbstractExpression Value { get; set; }
    }
}
