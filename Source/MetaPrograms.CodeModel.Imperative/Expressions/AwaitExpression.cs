using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Expressions
{
    public class AwaitExpression : AbstractExpression
    {
        public AwaitExpression(
            MemberRef<TypeMember> type, 
            AbstractExpression expression) 
            : base(type)
        {
            Expression = expression;
        }

        public AwaitExpression(
            AwaitExpression source,
            Mutator<AbstractExpression>? expression = null, 
            Mutator<MemberRef<TypeMember>>? type = null) 
            : base(source, type)
        {
            Expression = expression.MutatedOrOriginal(source.Expression);
        }

        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitAwaitExpression(this);

            if (this.Type.Get() != null)
            {
                visitor.VisitReferenceToTypeMember(this.Type.Get());
            }

            if (Expression != null)
            {
                Expression.AcceptVisitor(visitor);
            }
        }

        public AbstractExpression Expression { get; }
    }
}
