using MetaPrograms.Fluent;
using MetaPrograms.Members;

namespace MetaPrograms.Expressions
{
    public class MemberExpression : AbstractExpression, IAssignable
    {
        public AbstractExpression AsExpression()
        {
            return this;
        }

        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitMemberExpression(this);

            if (Target != null)
            {
                Target.AcceptVisitor(visitor);
            }
        }

        public override AbstractExpression AcceptRewriter(StatementRewriter rewriter)
        {
            return rewriter.RewriteMemberExpression(this);
        }

        IAssignable IAssignable.AcceptRewriter(StatementRewriter rewriter)
        {
            return (IAssignable)this.AcceptRewriter(rewriter);
        }

        public AbstractExpression Target { get; set; }
        public AbstractMember Member { get; set; }
        public IdentifierName MemberName { get; set; }
        public IdentifierName Name => MemberName ?? Member?.Name;

        public static MemberExpression Create(AbstractExpression target, AbstractMember member)
        {
            return BlockContext.GetBlockOrThrow().PushExpression(new MemberExpression { 
                Type = target.Type, 
                Target = target, 
                Member = member
            });
        }

        public static MemberExpression Create(AbstractExpression target, IdentifierName memberName)
        {
            return BlockContext.GetBlockOrThrow().PushExpression(new MemberExpression {
                Type = target.Type,
                Target = target,
                Member = null,
                MemberName = memberName
            });
        }
    }
}
