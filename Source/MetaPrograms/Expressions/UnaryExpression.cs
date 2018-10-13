using MetaPrograms.Members;

namespace MetaPrograms.Expressions
{
    public class UnaryExpression : AbstractExpression
    {
        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitUnaryExpression(this);

            if (Operand != null)
            {
                Operand.AcceptVisitor(visitor);
            }
        }

        public override AbstractExpression AcceptRewriter(StatementRewriter rewriter)
        {
            return rewriter.RewriteUnaryExpression(this);
        }

        public UnaryOperator Operator { get; set; }
        public AbstractExpression Operand { get; set; }
    }
}
