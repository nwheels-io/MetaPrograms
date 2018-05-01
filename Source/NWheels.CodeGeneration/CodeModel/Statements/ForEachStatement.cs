using NWheels.CodeGeneration.CodeModel.Expressions;
using NWheels.CodeGeneration.CodeModel.Members;

namespace NWheels.CodeGeneration.CodeModel.Statements
{
    public class ForEachStatement : AbstractStatement
    {
        public ForEachStatement()
        {
            this.Body = new BlockStatement();
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitForEachStatement(this);

            if (Enumerable != null)
            {
                Enumerable.AcceptVisitor(visitor);
            }

            Body.AcceptVisitor(visitor);

            if (CurrentItem != null)
            {
                visitor.VisitReferenceToLocalVariable(CurrentItem);
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public AbstractExpression Enumerable { get; set; }
        public BlockStatement Body { get; }
        public LocalVariable CurrentItem { get; set; }
    }
}
