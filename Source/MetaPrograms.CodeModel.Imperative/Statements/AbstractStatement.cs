namespace MetaPrograms.CodeModel.Imperative.Statements
{
    public abstract class AbstractStatement
    {
        public BindingCollection Bindings { get; set; } = new BindingCollection();
        public abstract void AcceptVisitor(StatementVisitor visitor);
        public abstract AbstractStatement AcceptRewriter(StatementRewriter rewriter);
    }
}
