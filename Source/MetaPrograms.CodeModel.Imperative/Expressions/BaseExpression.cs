using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Expressions
{
    public class BaseExpression : AbstractExpression
    {
        public BaseExpression(TypeMember type) 
            : base(type)
        {
        }

        public BaseExpression(
            BaseExpression expression, 
            Mutator<TypeMember>? type = null) 
            : base(expression, type)
        {
        }

        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitBaseExpression(this);
        }
    }
}
