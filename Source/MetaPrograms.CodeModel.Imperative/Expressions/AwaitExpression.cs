using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Expressions
{
    public class AwaitExpression : AbstractExpression
    {
        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitAwaitExpression(this);

            if (this.Type != null)
            {
                visitor.VisitReferenceToTypeMember(this.Type);
            }

            if (Expression != null)
            {
                Expression.AcceptVisitor(visitor);
            }
        }

        public AbstractExpression Expression { get; set; }
    }
}
