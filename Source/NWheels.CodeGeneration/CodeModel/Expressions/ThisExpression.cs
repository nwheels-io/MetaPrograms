namespace NWheels.CodeGeneration.CodeModel.Expressions
{
    public class ThisExpression : AbstractExpression
    {
        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitThisExpression(this);
        }
    }
}
