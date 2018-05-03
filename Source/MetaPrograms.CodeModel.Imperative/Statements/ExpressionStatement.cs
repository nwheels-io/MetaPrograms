using MetaPrograms.CodeModel.Imperative.Expressions;

namespace MetaPrograms.CodeModel.Imperative.Statements
{
    public class ExpressionStatement : AbstractStatement
    {
        public ExpressionStatement(AbstractExpression expression)
        {
            Expression = expression;
        }

        public ExpressionStatement(
            ExpressionStatement source, 
            Mutator<AbstractExpression>? expression = null)
        {
            Expression = expression.MutatedOrOriginal(source.Expression);
        }

        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitExpressionStatement(this);

            if (Expression != null)
            {
                Expression.AcceptVisitor(visitor);
            }
        }

        public AbstractExpression Expression { get; }
    }
}
