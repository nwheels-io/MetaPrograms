using MetaPrograms.CodeModel.Imperative.Expressions;

namespace MetaPrograms.CodeModel.Imperative.Statements
{
    public class ReturnStatement : AbstractStatement
    {
        public ReturnStatement(AbstractExpression expression)
        {
            Expression = expression;
        }

        public ReturnStatement(
            ReturnStatement source,
            Mutator<AbstractExpression>? expression = null)
        {
            Expression = expression.MutatedOrOriginal(source.Expression);
        }

        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitReturnStatement(this);

            if (Expression != null)
            {
                Expression.AcceptVisitor(visitor);
            }
        }

        public AbstractExpression Expression { get; }
    }
}
