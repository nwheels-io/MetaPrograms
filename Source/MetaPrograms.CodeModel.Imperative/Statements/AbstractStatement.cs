namespace MetaPrograms.CodeModel.Imperative.Statements
{
    public abstract class AbstractStatement
    {
        public BindingCollection Bindings { get; } = new BindingCollection();
        public abstract void AcceptVisitor(StatementVisitor visitor);
    }
}
