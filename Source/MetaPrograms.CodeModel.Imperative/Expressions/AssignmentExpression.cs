using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Expressions
{
    public class AssignmentExpression : AbstractExpression
    {
        public AssignmentExpression(
            IAssignable left, 
            AbstractExpression right) 
            : base(right.Type)
        {
            this.Left = left;
            this.Right = right;
        }

        public AssignmentExpression(
            AssignmentExpression expression,
            Mutator<IAssignable>? left = null,
            Mutator<AbstractExpression>? right = null) 
            : base(expression, right.MutatedOrOriginal(expression.Right).Type)
        {
            this.Left = left.MutatedOrOriginal(expression.Left);
            this.Right = right.MutatedOrOriginal(expression.Right);
        }

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

        public IAssignable Left { get; }
        public AbstractExpression Right { get; }
    }
}
