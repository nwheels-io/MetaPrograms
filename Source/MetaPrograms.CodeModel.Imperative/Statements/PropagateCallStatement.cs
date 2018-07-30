using MetaPrograms.CodeModel.Imperative.Expressions;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Statements
{
    public class PropagateCallStatement : AbstractStatement
    {
        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitPropagateCallStatement(this);

            if (Target != null)
            {
                Target.AcceptVisitor(visitor);
            }

            if (ReturnValue != null)
            {
                visitor.VisitReferenceToLocalVariable(ReturnValue);
            }
        }

        public override AbstractStatement AcceptRewriter(StatementRewriter rewriter)
        {
            return rewriter.RewritePropagateCallStatement(this);
        }

        public AbstractExpression Target { get; set; }

        //TODO: what is this variable for?
        public LocalVariable ReturnValue { get; set; } 
    }
}
