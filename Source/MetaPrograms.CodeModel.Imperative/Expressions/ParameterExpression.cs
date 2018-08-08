using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Expressions
{
    public class ParameterExpression : AbstractExpression, IAssignable
    {
        public AbstractExpression AsExpression()
        {
            return this;
        }

        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitParameterExpression(this);
        }

        public override AbstractExpression AcceptRewriter(StatementRewriter rewriter)
        {
            return rewriter.RewriteParameterExpression(this);
        }

        IAssignable IAssignable.AcceptRewriter(StatementRewriter rewriter)
        {
            return (IAssignable)this.AcceptRewriter(rewriter);
        }

        public MethodParameter Parameter { get; set; }
        public IdentifierName Name => Parameter?.Name;
    }
}
