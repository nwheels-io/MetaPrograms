using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Expressions
{
    public class AssignmentExpression : AbstractExpression
    {
        public AssignmentExpression(
            AbstractExpression left, 
            AbstractExpression right) 
            : base(left.Type)
        {
            this.Left = left;
            this.Right = right;
        }

        public AssignmentExpression(
            AssignmentExpression expression,
            Mutator<AbstractExpression>? left = null,
            Mutator<AbstractExpression>? right = null) 
            : base(expression, left.MutatedOrOriginal(expression.Left).Type)
        {
            this.Left = left.MutatedOrOriginal(expression.Left);
            this.Right = right.MutatedOrOriginal(expression.Right);
        }

        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitAssignmentExpression(this);

            if (Left != null)
            {
                Left.AcceptVisitor(visitor);
            }

            if (Right != null)
            {
                Right.AcceptVisitor(visitor);
            }
        }

        public AbstractExpression Left { get; }
        public AbstractExpression Right { get; }
    }
}
