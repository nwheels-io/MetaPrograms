namespace MetaPrograms.Statements
{
    public class ReThrowStatement : AbstractStatement
    {
        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitReThrowStatement(this);
        }

        public override AbstractStatement AcceptRewriter(StatementRewriter rewriter)
        {
            return rewriter.RewriteReThrowStatement(this);
        }
    }
}
