namespace NWheels.CodeGeneration.CodeModel.Statements
{
    public abstract class AbstractStatement
    {
        public abstract void AcceptVisitor(StatementVisitor visitor);
    }
}
