using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Expressions
{
    public class BaseExpression : AbstractExpression
    {
        public BaseExpression(MemberRef<TypeMember> type) 
            : base(type)
        {
        }

        public BaseExpression(
            BaseExpression expression, 
            Mutator<MemberRef<TypeMember>>? type = null) 
            : base(expression, type)
        {
        }

        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitBaseExpression(this);
        }
    }
}
