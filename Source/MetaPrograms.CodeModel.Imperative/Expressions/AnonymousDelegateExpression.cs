using System;
using MetaPrograms.CodeModel.Imperative.Members;
using MetaPrograms.CodeModel.Imperative.Statements;

namespace MetaPrograms.CodeModel.Imperative.Expressions
{
    public class AnonymousDelegateExpression : AbstractExpression, IFunctionContext
    {
        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitAnonymousDelegateExpression(this);
            Body.AcceptVisitor(visitor);
        }

        public override AbstractExpression AcceptRewriter(StatementRewriter rewriter)
        {
            return rewriter.RewriteAnonymousDelegateExpression(this);
        }

        public MethodSignature Signature { get; set; }
        public BlockStatement Body { get; set; }
    }
}
