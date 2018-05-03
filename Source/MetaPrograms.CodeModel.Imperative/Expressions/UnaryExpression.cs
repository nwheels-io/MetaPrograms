using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Expressions
{
    public class UnaryExpression : AbstractExpression
    {
        public UnaryExpression(
            TypeMember type, 
            UnaryOperator @operator, 
            AbstractExpression operand) 
            : base(type)
        {
            Operator = @operator;
            Operand = operand;
        }

        public UnaryExpression(
            UnaryExpression source,
            Mutator<AbstractExpression>? operand = null,
            Mutator<UnaryOperator>? @operator = null, 
            Mutator<TypeMember>? type = null) 
            : base(source, type)
        {
            Operator = @operator.MutatedOrOriginal(source.Operator);
            Operand = operand.MutatedOrOriginal(source.Operand);
        }

        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitUnaryExpression(this);

            if (Operand != null)
            {
                Operand.AcceptVisitor(visitor);
            }
        }

        public UnaryOperator Operator { get; }
        public AbstractExpression Operand { get; }
    }
}
