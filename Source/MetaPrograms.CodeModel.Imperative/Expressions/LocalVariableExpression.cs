using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Expressions
{
    public class LocalVariableExpression : AbstractExpression, IAssignable
    {
        public AbstractExpression AsExpression()
        {
            return this;
        }

        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitLocalVariableExpression(this);

            if (Variable != null)
            {
                visitor.VisitReferenceToLocalVariable(Variable);
            }
        }

        public override AbstractExpression AcceptRewriter(StatementRewriter rewriter)
        {
            return rewriter.RewriteLocalVariableExpression(this);
        }

        IAssignable IAssignable.AcceptRewriter(StatementRewriter rewriter)
        {
            return (IAssignable)this.AcceptRewriter(rewriter);
        }

        public LocalVariable Variable { get; set; }
        public IdentifierName VariableName { get; set; }

        public IdentifierName Name => VariableName ?? Variable?.Name;
    }
}
