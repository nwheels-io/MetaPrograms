using MetaPrograms.Expressions;

namespace MetaPrograms.Statements
{
    public class ExpressionStatement : AbstractStatement
    {
        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitExpressionStatement(this);

            if (Expression != null)
            {
                Expression.AcceptVisitor(visitor);
            }
        }

        public override AbstractStatement AcceptRewriter(StatementRewriter rewriter)
        {
            return rewriter.RewriteExpressionStatement(this);
        }

        public AbstractExpression Expression { get; set; }
    }
}
