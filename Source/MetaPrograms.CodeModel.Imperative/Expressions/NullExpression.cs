namespace MetaPrograms.CodeModel.Imperative.Expressions
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
