namespace MetaPrograms.Expressions
{
    public class NullExpression : AbstractExpression
    {
        public override void AcceptVisitor(StatementVisitor visitor)
        {
        }

        public override AbstractExpression AcceptRewriter(StatementRewriter rewriter)
        {
            return rewriter.RewriteNullExpression(this);
        }
    }
}
