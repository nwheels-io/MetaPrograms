using MetaPrograms.Expressions;

namespace MetaPrograms.Statements
{
    public class UsingStatement : AbstractStatement
    {
        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitUsingStatement(this);

            if (Disposable != null)
            {
                Disposable.AcceptVisitor(visitor);
            }

            Body.AcceptVisitor(visitor);
        }

        public override AbstractStatement AcceptRewriter(StatementRewriter rewriter)
        {
            return rewriter.RewriteUsingStatement(this);
        }

        public AbstractExpression Disposable { get; set; }
        public BlockStatement Body { get; set; }
    }
}
