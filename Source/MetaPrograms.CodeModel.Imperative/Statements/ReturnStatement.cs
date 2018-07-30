using MetaPrograms.CodeModel.Imperative.Expressions;

namespace MetaPrograms.CodeModel.Imperative.Statements
{
    public class ReturnStatement : AbstractStatement
    {
        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitReturnStatement(this);

            if (Expression != null)
            {
                Expression.AcceptVisitor(visitor);
            }
        }

        public override AbstractStatement AcceptRewriter(StatementRewriter rewriter)
        {
            return rewriter.RewriteReturnStatement(this);
        }

        public AbstractExpression Expression { get; set; }
    }
}
