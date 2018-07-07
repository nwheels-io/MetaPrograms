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

        public LocalVariable Variable { get; set; }
    }
}
