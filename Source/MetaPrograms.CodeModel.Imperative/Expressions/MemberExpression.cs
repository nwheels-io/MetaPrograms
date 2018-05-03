using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Expressions
{
    public class MemberExpression : AbstractExpression
    {
        public MemberExpression(
            TypeMember type, 
            AbstractExpression target, 
            AbstractMember member) : base(type)
        {
            Target = target;
            Member = member;
        }

        public MemberExpression(
            MemberExpression source,
            Mutator<TypeMember>? type = null,
            Mutator<AbstractExpression>? target = null,
            Mutator<AbstractMember>? member = null) 
            : base(source, type)
        {
            Target = target.MutatedOrOriginal(source.Target);
            Member = member.MutatedOrOriginal(source.Member);
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
        public AbstractMember Member { get; }
    }
}
