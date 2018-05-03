using MetaPrograms.CodeModel.Imperative.Expressions;

namespace MetaPrograms.CodeModel.Imperative.Statements
{
    public class LockStatement : AbstractStatement
    {
        public LockStatement(AbstractExpression syncRoot, AbstractExpression enterTimeout, BlockStatement body)
        {
            SyncRoot = syncRoot;
            EnterTimeout = enterTimeout;
            Body = body;
        }

        public LockStatement(
            LockStatement source,
            Mutator<AbstractExpression>? syncRoot = null,
            Mutator<AbstractExpression>? enterTimeout = null,
            Mutator<BlockStatement>? body = null)
        {
            SyncRoot = syncRoot.MutatedOrOriginal(source.SyncRoot);
            EnterTimeout = enterTimeout.MutatedOrOriginal(source.EnterTimeout);
            Body = body.MutatedOrOriginal(source.Body);
        }

        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitLockStatement(this);

            if (SyncRoot != null)
            {
                SyncRoot.AcceptVisitor(visitor);
            }

            if (EnterTimeout != null)
            {
                EnterTimeout.AcceptVisitor(visitor);
            }

            if (Body != null)
            {
                Body.AcceptVisitor(visitor);
            }
        }

        public AbstractExpression SyncRoot { get; }
        public AbstractExpression EnterTimeout { get; }
        public BlockStatement Body { get; }
    }
}
