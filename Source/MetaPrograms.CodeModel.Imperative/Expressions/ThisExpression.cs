using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Expressions
{
    public class ThisExpression : AbstractExpression
    {
        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitThisExpression(this);
        }

        public override AbstractExpression AcceptRewriter(StatementRewriter rewriter)
        {
            return rewriter.RewriteThisExpression(this);
        }
    }
}
