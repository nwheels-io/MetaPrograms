using MetaPrograms.Expressions;

namespace MetaPrograms.Statements
{
    public class DoStatement : AbstractStatement
    {
        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitDoStatement(this);

            Body.AcceptVisitor(visitor);

            if (Condition != null)
            {
                Condition.AcceptVisitor(visitor);
            }
        }

        public override AbstractStatement AcceptRewriter(StatementRewriter rewriter)
        {
            return rewriter.RewriteDoStatement(this);
        }

        public BlockStatement Body { get; set; }
        public AbstractExpression Condition { get; set; }
    }
}
