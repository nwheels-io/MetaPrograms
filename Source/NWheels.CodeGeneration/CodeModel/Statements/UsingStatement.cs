using NWheels.CodeGeneration.CodeModel.Expressions;

namespace NWheels.CodeGeneration.CodeModel.Statements
{
    public class UsingStatement : AbstractStatement
    {
        public UsingStatement()
        {
            this.Body = new BlockStatement();
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitUsingStatement(this);

            if (Disposable != null)
            {
                Disposable.AcceptVisitor(visitor);
            }

            Body.AcceptVisitor(visitor);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public AbstractExpression Disposable { get; set; }
        public BlockStatement Body { get; }
    }
}
