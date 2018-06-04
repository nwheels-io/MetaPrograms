namespace MetaPrograms.CodeModel.Imperative.Expressions
{
    public interface IAssignable
    {
        AbstractExpression AsExpression();
        void AcceptVisitor(StatementVisitor visitor);
    }
}
