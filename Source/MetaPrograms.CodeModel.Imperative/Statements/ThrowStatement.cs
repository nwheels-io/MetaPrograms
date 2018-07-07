﻿using MetaPrograms.CodeModel.Imperative.Expressions;

namespace MetaPrograms.CodeModel.Imperative.Statements
{
    public class ThrowStatement : AbstractStatement
    {
        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitThrowStatement(this);

            if (Exception != null)
            {
                Exception.AcceptVisitor(visitor);
            }
        }

        public AbstractExpression Exception { get; set; }
    }
}
