using NWheels.CodeGeneration.CodeModel.Expressions;
using NWheels.CodeGeneration.CodeModel.Members;

namespace NWheels.CodeGeneration.CodeModel.Statements
{
    public class PropagateCallStatement : AbstractStatement
    {
        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitPropagateCallStatement(this);

            if (Target != null)
            {
                Target.AcceptVisitor(visitor);
            }

            if (ReturnValue != null)
            {
                visitor.VisitReferenceToLocalVariable(ReturnValue);
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public AbstractExpression Target { get; set; }

        //TODO: what is this variable for?
        public LocalVariable ReturnValue { get; set; } 
    }
}
