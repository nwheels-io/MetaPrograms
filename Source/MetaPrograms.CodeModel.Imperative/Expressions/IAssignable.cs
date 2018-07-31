namespace MetaPrograms.CodeModel.Imperative.Expressions
{
    public interface IAssignable
    {
        string Name { get; }
        AbstractExpression AsExpression();
        void AcceptVisitor(StatementVisitor visitor);
        IAssignable AcceptRewriter(StatementRewriter rewriter);
    }
}
