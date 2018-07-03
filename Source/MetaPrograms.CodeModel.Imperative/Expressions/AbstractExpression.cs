using System;
using System.Collections;
using System.Collections.Immutable;
using System.Linq;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Expressions
{
    public abstract class AbstractExpression
    {
        protected AbstractExpression(MemberRef<TypeMember> type)
        {
            Type = type;
        }

        protected AbstractExpression(
            AbstractExpression expression, 
            Mutator<MemberRef<TypeMember>>? type = null)
        {
            Type = type.MutatedOrOriginal(expression.Type);
        }

        public abstract void AcceptVisitor(StatementVisitor visitor);

        public BindingCollection Bindings { get; } = new BindingCollection();
        public MemberRef<TypeMember> Type { get; }

        public static AbstractExpression FromValue(object value)
        {
            return FromValue(value, resolveType: t => MemberRef<TypeMember>.Null);
        }

        public static AbstractExpression FromValue(object value, Func<Type, MemberRef<TypeMember>> resolveType)
        {
            if (value == null)
            {
                return new ConstantExpression(MemberRef<TypeMember>.Null, null);
            }

            if (value is AbstractExpression expr)
            {
                return expr;
            }

            var typeRef = resolveType(value.GetType());
            var type = typeRef.Get();
            
            if (type != null && type.IsArray)
            {
                return new NewArrayExpression(
                    typeRef, 
                    type.UnderlyingType, 
                    FromValue(((IList)value).Count, resolveType),
                    ((IEnumerable)value).Cast<object>().Select(x => FromValue(x, resolveType)).ToImmutableList());
            }
            
            return new ConstantExpression(typeRef, value);
        }
    }
}
