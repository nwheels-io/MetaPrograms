using MetaPrograms.Expressions;

namespace MetaPrograms.Statements
{
    public class ThrowStatement : AbstractStatement
    {
        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitThrowStatement(this);

            if (Exception != null)
            {
                Exception.AcceptVisitor(visitor);
            }
        }

        public override AbstractStatement AcceptRewriter(StatementRewriter rewriter)
        {
            return rewriter.RewriteThrowStatement(this);
        }

        public AbstractExpression Exception { get; set; }
    }
}
