using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Expressions
{
    public class NewObjectExpression : AbstractExpression
    {
        public NewObjectExpression(
            MethodCallExpression constructorCall) 
            : base(constructorCall.Type)
        {
            ConstructorCall = constructorCall;
        }

        public NewObjectExpression(
            NewObjectExpression source,
            Mutator<MethodCallExpression>? constructorCall = null) 
            : base(source, constructorCall.MutatedOrOriginal(source.ConstructorCall).Type)
        {
            ConstructorCall = constructorCall.MutatedOrOriginal(source.ConstructorCall);
        }

        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitNewObjectExpression(this);

            if (ConstructorCall != null)
            {
                ConstructorCall.AcceptVisitor(visitor);
            }
        }

        public MethodCallExpression ConstructorCall { get; }
    }
}
