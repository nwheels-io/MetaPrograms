﻿using MetaPrograms.CodeModel.Imperative.Expressions;

namespace MetaPrograms.CodeModel.Imperative.Statements
{
    public class LockStatement : AbstractStatement
    {
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

        public AbstractExpression SyncRoot { get; set; }
        public AbstractExpression EnterTimeout { get; set; }
        public BlockStatement Body { get; set; }
    }
}
