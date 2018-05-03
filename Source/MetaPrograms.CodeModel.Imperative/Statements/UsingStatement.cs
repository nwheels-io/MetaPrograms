using MetaPrograms.CodeModel.Imperative.Expressions;

namespace MetaPrograms.CodeModel.Imperative.Statements
{
    public class UsingStatement : AbstractStatement
    {
        public UsingStatement(
            AbstractExpression disposable, 
            BlockStatement body)
        {
            Disposable = disposable;
            Body = body;
        }

        public UsingStatement(
            UsingStatement source,
            Mutator<AbstractExpression>? disposable = null,
            Mutator<BlockStatement>? body = null)
        {
            Disposable = disposable.MutatedOrOriginal(source.Disposable);
            Body = body.MutatedOrOriginal(source.Body);
        }

        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitUsingStatement(this);

            if (Disposable != null)
            {
                Disposable.AcceptVisitor(visitor);
            }

            Body.AcceptVisitor(visitor);
        }

        public AbstractExpression Disposable { get; }
        public BlockStatement Body { get; }
    }
}
