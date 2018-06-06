using System.Collections.Immutable;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Expressions
{
    public abstract class InvocationExpression : AbstractExpression
    {
        protected InvocationExpression(MemberRef<TypeMember> type, ImmutableList<Argument> arguments)
            : base(type)
        {
            this.Arguments = arguments;
        }

        protected InvocationExpression(
            InvocationExpression source, 
            Mutator<MemberRef<TypeMember>>? type = null,
            Mutator<ImmutableList<Argument>>? arguments = null)
            : base(source, type)
        {
            this.Arguments = arguments.MutatedOrOriginal(source.Arguments);
        }

        public ImmutableList<Argument> Arguments { get; }
    }
}
