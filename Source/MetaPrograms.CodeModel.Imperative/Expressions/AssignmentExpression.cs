using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Expressions
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

        public IAssignable Left { get; set; }
        public AbstractExpression Right { get; set; }
    }
}
