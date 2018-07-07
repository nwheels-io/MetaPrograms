using System;
using MetaPrograms.CodeModel.Imperative.Members;
using MetaPrograms.CodeModel.Imperative.Statements;

namespace MetaPrograms.CodeModel.Imperative.Expressions
{
    public class AnonymousDelegateExpression : AbstractExpression
    {
        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitAnonymousDelegateExpression(this);
            Body.AcceptVisitor(visitor);
        }

        public MethodSignature Signature { get; set; }
        public BlockStatement Body { get; set; }
    }
}
