﻿using MetaPrograms.CodeModel.Imperative.Fluent;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Expressions
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

        public AbstractExpression Target { get; set; }
        public AbstractMember Member { get; set; }
        public string MemberName { get; set; }

        public static MemberExpression Create(AbstractExpression target, AbstractMember member)
        {
            return BlockContext.GetBlockOrThrow().PushExpression(new MemberExpression { 
                Type = target.Type, 
                Target = target, 
                Member = member
            });
        }

        public static MemberExpression Create(AbstractExpression target, string memberName)
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
