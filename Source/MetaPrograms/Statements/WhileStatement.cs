using MetaPrograms.Expressions;

namespace MetaPrograms.Statements
{
    public class WhileStatement : AbstractStatement
    {
        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitWhileStatement(this);
            
            if (Condition != null)
            {
                Condition.AcceptVisitor(visitor);
            }

            Body.AcceptVisitor(visitor);
        }

        public override AbstractStatement AcceptRewriter(StatementRewriter rewriter)
        {
            return rewriter.RewriteWhileStatement(this);
        }

        public AbstractExpression Condition { get; set; }
        public BlockStatement Body { get; set; }
    }
}
