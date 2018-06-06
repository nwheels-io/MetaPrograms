using System.Collections.Generic;
using System.Collections.Immutable;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Expressions
{
    public class MethodCallExpression : InvocationExpression
    {
        public MethodCallExpression(
            MemberRef<TypeMember> type, 
            AbstractExpression target,
            MemberRef<MethodMemberBase> method, 
            ImmutableList<Argument> arguments, 
            string methodName = null) 
            : base(type, arguments)
        {
            Target = target;
            Method = method;
            MethodName = methodName;
        }

        public MethodCallExpression(
            MethodCallExpression source,
            Mutator<AbstractExpression>? target = null,
            Mutator<MemberRef<TypeMember>>? type = null,
            Mutator<MemberRef<MethodMemberBase>>? method = null,
            Mutator<ImmutableList<Argument>>? arguments = null,
            Mutator<string>? methodName = null) 
            : base(source, type, arguments)
        {
            Target = target.MutatedOrOriginal(source.Target);
            Method = method.MutatedOrOriginal(source.Method);
            MethodName = methodName.MutatedOrOriginal(source.MethodName);
        }

        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitMethodCallExpression(this);

            if (Target != null)
            {
                Target.AcceptVisitor(visitor);
            }

            foreach (var argument in Arguments)
            {
                argument.Expression.AcceptVisitor(visitor);
            }
        }

        public AbstractExpression Target { get; }
        public MemberRef<MethodMemberBase> Method { get; }
        public string MethodName { get; }
    }
}

