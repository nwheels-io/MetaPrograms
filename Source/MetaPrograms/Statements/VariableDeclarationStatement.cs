using MetaPrograms.Expressions;
using MetaPrograms.Members;

namespace MetaPrograms.Statements
{
    public class VariableDeclarationStatement : AbstractStatement
    {
        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitVariableDeclaraitonStatement(this);

            if (Variable != null)
            {
                visitor.VisitReferenceToLocalVariable(Variable);
            }

            if (InitialValue != null)
            {
                InitialValue.AcceptVisitor(visitor);
            }
        }

        public override AbstractStatement AcceptRewriter(StatementRewriter rewriter)
        {
            return rewriter.RewriteVariableDeclaraitonStatement(this);
        }

        public LocalVariable Variable { get; set; }
        public AbstractExpression InitialValue { get; set; }
    }
}
