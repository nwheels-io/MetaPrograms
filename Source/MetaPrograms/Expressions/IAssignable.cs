namespace MetaPrograms.Expressions
{
    public interface IAssignable
    {
        IdentifierName Name { get; }
        AbstractExpression AsExpression();
        void AcceptVisitor(StatementVisitor visitor);
        IAssignable AcceptRewriter(StatementRewriter rewriter);
    }
}
