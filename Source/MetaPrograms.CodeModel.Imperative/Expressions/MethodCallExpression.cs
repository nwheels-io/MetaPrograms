using System.Collections.Generic;
using System.Collections.Immutable;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Expressions
{
    public class MethodCallExpression : AbstractExpression
    {
        public MethodCallExpression(
            TypeMember type, 
            AbstractExpression target, 
            MethodMember method, 
            ImmutableList<Argument> arguments, 
            bool isAsyncAwait) : base(type)
        {
            Target = target;
            Method = method;
            Arguments = arguments;
            IsAsyncAwait = isAsyncAwait;
        }

        public MethodCallExpression(
            MethodCallExpression source,
            Mutator<AbstractExpression>? target = null,
            Mutator<TypeMember>? type = null,
            Mutator<MethodMember>? method = null,
            Mutator<ImmutableList<Argument>>? arguments = null,
            Mutator<bool>? isAsyncAwait = null) 
            : base(source, type)
        {
            Target = target.MutatedOrOriginal(source.Target);
            Method = method.MutatedOrOriginal(source.Method);
            Arguments = arguments.MutatedOrOriginal(source.Arguments);
            IsAsyncAwait = isAsyncAwait.MutatedOrOriginal(source.IsAsyncAwait);
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
        public MethodMember Method { get; }
        public ImmutableList<Argument> Arguments { get; }
        public bool IsAsyncAwait { get; }
    }
}

