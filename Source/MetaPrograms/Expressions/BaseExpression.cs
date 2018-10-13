using MetaPrograms.Members;

namespace MetaPrograms.Expressions
{
    public class BaseExpression : AbstractExpression
    {
        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitBaseExpression(this);
        }

        public override AbstractExpression AcceptRewriter(StatementRewriter rewriter)
        {
            return rewriter.RewriteBaseExpression(this);
        }
    }
}
