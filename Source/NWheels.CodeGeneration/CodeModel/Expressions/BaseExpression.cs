namespace NWheels.CodeGeneration.CodeModel.Expressions
{
    public class BaseExpression : AbstractExpression
    {
        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitBaseExpression(this);
        }
    }
}
