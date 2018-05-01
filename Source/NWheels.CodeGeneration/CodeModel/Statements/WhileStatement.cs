using NWheels.CodeGeneration.CodeModel.Expressions;

namespace NWheels.CodeGeneration.CodeModel.Statements
{
    public class WhileStatement : AbstractStatement
    {
        public WhileStatement()
        {
            this.Body = new BlockStatement();
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitWhileStatement(this);
            
            if (Condition != null)
            {
                Condition.AcceptVisitor(visitor);
            }

            Body.AcceptVisitor(visitor);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public AbstractExpression Condition { get; set; }
        public BlockStatement Body { get; set; }
    }
}
