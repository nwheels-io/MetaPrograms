using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Expressions
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
