using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Expressions
{
    public class ThisExpression : AbstractExpression
    {
        public ThisExpression(MemberRef<TypeMember> type) 
            : base(type)
        {
        }

        public ThisExpression(
            ThisExpression source, 
            Mutator<MemberRef<TypeMember>>? type = null) 
            : base(source, type)
        {
        }

        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitThisExpression(this);
        }
    }
}
