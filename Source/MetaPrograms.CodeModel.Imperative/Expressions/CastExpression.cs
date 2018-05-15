﻿using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Expressions
{
    public class CastExpression : AbstractExpression
    {
        public CastExpression(
            MemberRef<TypeMember> type, 
            AbstractExpression expression) 
            : base(type)
        {
            Expression = expression;
        }

        public CastExpression(
            CastExpression source,
            Mutator<AbstractExpression>? expression = null, 
            Mutator<MemberRef<TypeMember>>? type = null) 
            : base(source, type)
        {
            Expression = expression.MutatedOrOriginal(source.Expression);
        }

        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitCastExpression(this);

            if (this.Type.Get() != null)
            {
                visitor.VisitReferenceToTypeMember(this.Type.Get());
            }

            if (Expression != null)
            {
                Expression.AcceptVisitor(visitor);
            }
        }

        public AbstractExpression Expression { get; }
    }
}
