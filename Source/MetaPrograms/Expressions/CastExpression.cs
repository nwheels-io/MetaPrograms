using MetaPrograms.Members;

namespace MetaPrograms.Expressions
{
    public class CastExpression : AbstractExpression
    {
        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitCastExpression(this);

            if (this.Type != null)
            {
                visitor.VisitReferenceToTypeMember(this.Type);
            }

            Expression?.AcceptVisitor(visitor);
        }

        public override AbstractExpression AcceptRewriter(StatementRewriter rewriter)
        {
            return rewriter.RewriteCastExpression(this);
        }

        public AbstractExpression Expression { get; set; }
    }
}
