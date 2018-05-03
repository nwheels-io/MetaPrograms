using System.Collections.Generic;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Expressions
{
    public class BinaryExpression : AbstractExpression
    {
        public BinaryExpression(
            TypeMember type, 
            AbstractExpression left, 
            BinaryOperator @operator, 
            AbstractExpression right) 
            : base(type)
        {
            Left = left;
            Operator = @operator;
            Right = right;
        }

        public BinaryExpression(
            BinaryExpression expression,
            Mutator<TypeMember>? type = null,
            Mutator<AbstractExpression>? left = null,
            Mutator<BinaryOperator>? @operator = null,
            Mutator<AbstractExpression>? right = null) 
            : base(expression, type)
        {
            Left = left.MutatedOrOriginal(expression.Left);
            Operator = @operator.MutatedOrOriginal(expression.Operator);
            Right = right.MutatedOrOriginal(expression.Right);
        }

        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitBinaryExpression(this);

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
        public BinaryOperator Operator { get; }
        public AbstractExpression Right { get; }
    }
}
