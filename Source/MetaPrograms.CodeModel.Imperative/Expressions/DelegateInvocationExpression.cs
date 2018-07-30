using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Expressions
{
    public class DelegateInvocationExpression : InvocationExpression
    {
        public override void AcceptVisitor(StatementVisitor visitor)
        {
            this.Delegate?.AcceptVisitor(visitor);
        }

        public override AbstractExpression AcceptRewriter(StatementRewriter rewriter)
        {
            return rewriter.RewriteDelegateInvocationExpression(this);
        }

        public AbstractExpression Delegate { get; set; }
    }
}
