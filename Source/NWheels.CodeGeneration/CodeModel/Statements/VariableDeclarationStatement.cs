using NWheels.CodeGeneration.CodeModel.Expressions;
using NWheels.CodeGeneration.CodeModel.Members;

namespace NWheels.CodeGeneration.CodeModel.Statements
{
    public class VariableDeclarationStatement : AbstractStatement
    {
        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitVariableDeclaraitonStatement(this);

            if (Variable != null)
            {
                visitor.VisitReferenceToLocalVariable(Variable);
            }

            if (InitialValue != null)
            {
                InitialValue.AcceptVisitor(visitor);
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public LocalVariable Variable { get; set; }
        public AbstractExpression InitialValue { get; set; }
    }
}
