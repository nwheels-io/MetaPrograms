using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Expressions
{
    public class MemberExpression : AbstractExpression, IAssignable
    {
        public MemberExpression(
            MemberRef<TypeMember> type, 
            AbstractExpression target,
            MemberRef<AbstractMember> member,
            string memberName = null) : base(type)
        {
            Target = target;
            Member = member;
            MemberName = memberName;
        }

        public MemberExpression(
            MemberExpression source,
            Mutator<MemberRef<TypeMember>>? type = null,
            Mutator<AbstractExpression>? target = null,
            Mutator<MemberRef<AbstractMember>>? member = null,
            Mutator<string>? memberName = null) 
            : base(source, type)
        {
            Target = target.MutatedOrOriginal(source.Target);
            Member = member.MutatedOrOriginal(source.Member);
            MemberName = memberName.MutatedOrOriginal(source.MemberName);
        }

        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitMemberExpression(this);

            if (Target != null)
            {
                Target.AcceptVisitor(visitor);
            }
        }

        public AbstractExpression Target { get; }
        public MemberRef<AbstractMember> Member { get; }
        public string MemberName { get; }
    }
}
