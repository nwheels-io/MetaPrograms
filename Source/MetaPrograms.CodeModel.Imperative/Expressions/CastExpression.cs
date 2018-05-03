using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Expressions
{
    public class CastExpression : AbstractExpression
    {
        public CastExpression(
            TypeMember type, 
            AbstractExpression expression) 
            : base(type)
        {
            Expression = expression;
        }

        public CastExpression(
            CastExpression source,
            Mutator<AbstractExpression>? expression = null, 
            Mutator<TypeMember>? type = null) 
            : base(source, type)
        {
            Expression = expression.MutatedOrOriginal(source.Expression);
        }

        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitCastExpression(this);

            if (this.Type != null)
            {
                visitor.VisitReferenceToTypeMember(this.Type);
            }

            if (Expression != null)
            {
                Expression.AcceptVisitor(visitor);
            }
        }

        public AbstractExpression Expression { get; }
    }
}
