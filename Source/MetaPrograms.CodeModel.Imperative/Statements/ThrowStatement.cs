using MetaPrograms.CodeModel.Imperative.Expressions;

namespace MetaPrograms.CodeModel.Imperative.Statements
{
    public class ThrowStatement : AbstractStatement
    {
        public ThrowStatement(AbstractExpression exception)
        {
            Exception = exception;
        }

        public ThrowStatement(
            ThrowStatement source,
            Mutator<AbstractExpression>? exception = null)
        {
            Exception = exception.MutatedOrOriginal(source.Exception);
        }

        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitThrowStatement(this);

            if (Exception != null)
            {
                Exception.AcceptVisitor(visitor);
            }
        }

        public AbstractExpression Exception { get; }
    }
}
