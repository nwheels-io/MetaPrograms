using System.Collections.Generic;
using System.Collections.Immutable;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Expressions
{
    public class MethodCallExpression : AbstractExpression
    {
        public MethodCallExpression(
            MemberRef<TypeMember> type, 
            AbstractExpression target,
            MemberRef<MethodMember> method, 
            ImmutableList<Argument> arguments, 
            bool isAsyncAwait,
            string methodName = null) : base(type)
        {
            Target = target;
            Method = method;
            Arguments = arguments;
            IsAsyncAwait = isAsyncAwait;
            MethodName = methodName;
        }

        public MethodCallExpression(
            MethodCallExpression source,
            Mutator<AbstractExpression>? target = null,
            Mutator<MemberRef<TypeMember>>? type = null,
            Mutator<MemberRef<MethodMember>>? method = null,
            Mutator<ImmutableList<Argument>>? arguments = null,
            Mutator<bool>? isAsyncAwait = null,
            Mutator<string>? methodName = null) 
            : base(source, type)
        {
            Target = target.MutatedOrOriginal(source.Target);
            Method = method.MutatedOrOriginal(source.Method);
            Arguments = arguments.MutatedOrOriginal(source.Arguments);
            IsAsyncAwait = isAsyncAwait.MutatedOrOriginal(source.IsAsyncAwait);
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
        public MemberRef<MethodMember> Method { get; }
        public string MethodName { get; }
        public ImmutableList<Argument> Arguments { get; }
        public bool IsAsyncAwait { get; }
    }
}

