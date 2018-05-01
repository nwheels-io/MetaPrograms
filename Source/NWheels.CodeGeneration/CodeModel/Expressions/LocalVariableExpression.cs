using NWheels.CodeGeneration.CodeModel.Members;

namespace NWheels.CodeGeneration.CodeModel.Expressions
{
    public class LocalVariableExpression : AbstractExpression
    {
        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitLocalVariableExpression(this);

            if (Variable != null)
            {
                visitor.VisitReferenceToLocalVariable(Variable);
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public LocalVariable Variable { get; set; }
    }
}
