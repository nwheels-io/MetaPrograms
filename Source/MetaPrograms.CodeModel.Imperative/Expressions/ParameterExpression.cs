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

        public MethodParameter Parameter { get; set; }
    }
}
