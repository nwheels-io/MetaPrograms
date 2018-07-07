using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Expressions
{
    public abstract class AbstractExpression
    {
        public abstract void AcceptVisitor(StatementVisitor visitor);

        public BindingCollection Bindings { get; } = new BindingCollection();
        public TypeMember Type { get; set; }

        public static AbstractExpression FromValue(object value)
        {
            return FromValue(value, resolveType: t => null);
        }

        public static AbstractExpression FromValue(object value, Func<Type, TypeMember> resolveType)
        {
            if (value == null)
            {
                return new ConstantExpression {
                    Type = null,
                    Value = null
                };
            }

            if (value is AbstractExpression expr)
            {
                return expr;
            }

            var type = resolveType(value.GetType());
            
            if (type != null && type.IsArray)
            {
                return InitializedArrayAsConstantExpression(type, (IList)value, resolveType);
            }

            return new ConstantExpression {
                Type = type,
                Value = value
            };
        }

        private static AbstractExpression InitializedArrayAsConstantExpression(
            TypeMember arrayType,
            IList arrayObject, 
            Func<Type, TypeMember> resolveType)
        {
            var arrayItems = arrayObject
                .Cast<object>()
                .Select(x => FromValue(x, resolveType))
                .ToList();

            return new NewArrayExpression {
                Type = arrayType,
                ElementType = arrayType.UnderlyingType,
                Length = FromValue(arrayObject.Count, resolveType),
                DimensionInitializerValues = new List<List<AbstractExpression>> {arrayItems}
            };
        }
    }
}
