using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Expressions
{
    public class NewObjectExpression : AbstractExpression
    {
        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitNewObjectExpression(this);

            if (ConstructorCall != null)
            {
                ConstructorCall.AcceptVisitor(visitor);
            }
        }

        public MethodCallExpression ConstructorCall { get; set; }
    }
}
