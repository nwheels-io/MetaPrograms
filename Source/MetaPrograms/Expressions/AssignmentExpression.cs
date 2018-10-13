using MetaPrograms.Members;

namespace MetaPrograms.Expressions
{
    public class AssignmentExpression : AbstractExpression
    {
        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitAssignmentExpression(this);

            if (Left is AbstractExpression leftExpression)
            {
                Left.AcceptVisitor(visitor);
            }

            if (Right != null)
            {
                Right.AcceptVisitor(visitor);
            }
        }

        public override AbstractExpression AcceptRewriter(StatementRewriter rewriter)
        {
            return rewriter.RewriteAssignmentExpression(this);
        }

        public IAssignable Left { get; set; }
        public AbstractExpression Right { get; set; }
        public CompoundAssignmentOperator CompoundOperator { get; set; }
    }
}
