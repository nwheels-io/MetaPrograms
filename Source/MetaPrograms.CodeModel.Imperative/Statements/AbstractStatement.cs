namespace MetaPrograms.CodeModel.Imperative.Statements
{
    public abstract class AbstractStatement
    {
        public abstract void AcceptVisitor(StatementVisitor visitor);
    }
}
