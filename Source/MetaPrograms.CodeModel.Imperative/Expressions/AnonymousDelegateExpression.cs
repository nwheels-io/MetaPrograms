using System;
using MetaPrograms.CodeModel.Imperative.Members;
using MetaPrograms.CodeModel.Imperative.Statements;

namespace MetaPrograms.CodeModel.Imperative.Expressions
{
    public class AnonymousDelegateExpression : AbstractExpression
    {
        public AnonymousDelegateExpression(MethodSignature signature, BlockStatement body)
            : base(signature.ReturnValue?.Type)
        {
            this.Body = body;
        }

        public AnonymousDelegateExpression(
            AnonymousDelegateExpression expression,
            Mutator<MethodSignature>? signature = null,
            Mutator<BlockStatement>? body = null) : 
            base(
                expression, 
                signature.MutatedOrOriginal(expression.Signature).ReturnValue?.Type)
        {
            this.Signature = signature.MutatedOrOriginal(expression.Signature);
            this.Body = body.MutatedOrOriginal(expression.Body);
        }

        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitAnonymousDelegateExpression(this);
            Body.AcceptVisitor(visitor);
        }

        public MethodSignature Signature { get; set; }
        public BlockStatement Body { get; }
    }
}
