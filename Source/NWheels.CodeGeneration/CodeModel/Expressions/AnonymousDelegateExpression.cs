using NWheels.CodeGeneration.CodeModel.Members;
using NWheels.CodeGeneration.CodeModel.Statements;

namespace NWheels.CodeGeneration.CodeModel.Expressions
{
    public class AnonymousDelegateExpression : AbstractExpression
    {
        public AnonymousDelegateExpression()
        {
            this.Body = new BlockStatement();
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitAnonymousDelegateExpression(this);
            Body.AcceptVisitor(visitor);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public MethodSignature Signature { get; set; }
        public BlockStatement Body { get; }
    }
}
