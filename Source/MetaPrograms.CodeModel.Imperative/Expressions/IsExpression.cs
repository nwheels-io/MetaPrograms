using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Expressions
{
    public class IsExpression : AbstractExpression
    {
        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitIsExpression(this);

            if (Expression != null)
            {
                Expression.AcceptVisitor(visitor);
            }

            if (PatternMatchVariable != null)
            {
                visitor.VisitReferenceToLocalVariable(PatternMatchVariable);
            }
        }

        public override AbstractExpression AcceptRewriter(StatementRewriter rewriter)
        {
            return rewriter.RewriteIsExpression(this);
        }

        public AbstractExpression Expression { get; set; }
        public LocalVariable PatternMatchVariable { get; set; }
    }
}
