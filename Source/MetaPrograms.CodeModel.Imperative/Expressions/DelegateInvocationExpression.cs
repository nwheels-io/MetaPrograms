using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Expressions
{
    public class DelegateInvocationExpression : InvocationExpression
    {
        public DelegateInvocationExpression(
            MemberRef<TypeMember> type,
            ImmutableList<Argument> arguments, 
            AbstractExpression @delegate)
            : base(type, arguments)
        {
            this.Delegate = @delegate;
        }

        public DelegateInvocationExpression(
            DelegateInvocationExpression source, 
            Mutator<MemberRef<TypeMember>>? type = null, 
            Mutator<ImmutableList<Argument>>? arguments = null, 
            Mutator<AbstractExpression>? @delegate = null)
            : base(source, type, arguments)
        {
            this.Delegate = @delegate.MutatedOrOriginal(source.Delegate);
        }

        public override void AcceptVisitor(StatementVisitor visitor)
        {
            this.Delegate?.AcceptVisitor(visitor);
        }

        public AbstractExpression Delegate { get; }
    }
}
