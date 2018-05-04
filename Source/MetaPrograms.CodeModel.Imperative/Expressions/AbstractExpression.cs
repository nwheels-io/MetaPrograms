using System;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Expressions
{
    public abstract class AbstractExpression
    {
        protected AbstractExpression(TypeMember type)
        {
            Type = type;
        }

        protected AbstractExpression(
            AbstractExpression expression, 
            Mutator<TypeMember>? type = null)
        {
            Type = type.MutatedOrOriginal(expression.Type);
        }

        public abstract void AcceptVisitor(StatementVisitor visitor);

        public BindingCollection Bindings { get; } = new BindingCollection();
        public TypeMember Type { get; }
    }
}
