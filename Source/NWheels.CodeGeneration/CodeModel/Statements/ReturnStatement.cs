using NWheels.CodeGeneration.CodeModel.Expressions;

namespace NWheels.CodeGeneration.CodeModel.Statements
{
    public class ReturnStatement : AbstractStatement
    {
        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitReturnStatement(this);

            if (Expression != null)
            {
                Expression.AcceptVisitor(visitor);
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public AbstractExpression Expression { get; set; }
    }
}
