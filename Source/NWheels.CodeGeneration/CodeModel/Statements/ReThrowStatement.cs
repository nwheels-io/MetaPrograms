namespace NWheels.CodeGeneration.CodeModel.Statements
{
    public class ReThrowStatement : AbstractStatement
    {
        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitReThrowStatement(this);
        }
    }
}
