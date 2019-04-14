using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using MetaPrograms.Members;

namespace MetaPrograms.Expressions
{
    public abstract class AbstractExpression
    {
        public abstract void AcceptVisitor(StatementVisitor visitor);
        public abstract AbstractExpression AcceptRewriter(StatementRewriter rewriter);

        public BindingCollection Bindings { get; set; } = new BindingCollection();
        public TypeMember Type { get; set; }

        public static AbstractExpression FromValue(object value)
        {
            return FromValue(value, resolveType: t => null);
        }

        public static AbstractExpression FromValue(object value, Func<Type, TypeMember> resolveType)
        {
            if (value == null)
            {
                return new NullExpression();
            }

            if (value is AbstractExpression expr)
            {
                return expr;
            }

            var clrType = value.GetType();
            var type = resolveType(clrType);
            
            if (value is IList valueAsList)
            {
                return InitializedArrayAsConstantExpression(type, valueAsList, resolveType);
            }

            if (value is IDictionary dictionary)
            {
                return InitializedDictionaryAsConstantExpression(dictionary, resolveType);
            }

            if ((clrType.IsClass || (clrType.IsValueType && !clrType.IsPrimitive)) && !(value is string))
            {
                return InitializedObjectAsConstantExpression(type, value, resolveType);
            }

            return new ConstantExpression {
                Type = type,
                Value = value
            };
        }

        private static AbstractExpression InitializedDictionaryAsConstantExpression(
            IDictionary dictionary, 
            Func<Type, TypeMember> resolveType)
        {
            return new ObjectInitializerExpression {
                Type = null, // TODO: populate Type
                PropertyValues = dictionary.Keys
                    .Cast<object>()
                    .Select(key => new NamedPropertyValue {
                        Name = key.ToString(),
                        Value = FromValue(dictionary[key])
                    }).ToList()
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
                ElementType = arrayType?.UnderlyingType,
                Length = FromValue(arrayObject.Count, resolveType),
                DimensionInitializerValues = new List<List<AbstractExpression>> {arrayItems}
            };
        }

        private static AbstractExpression InitializedObjectAsConstantExpression(
            TypeMember objectType,
            object obj, 
            Func<Type, TypeMember> resolveType)
        {
            var properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var propertyValues = properties.Select(p => new NamedPropertyValue {
                //Member = objectType.Members.OfType<PropertyMember>().FirstOrDefault(m => m.Name == p.Name),
                Name = new IdentifierName(p.Name, language: null, style: CasingStyle.Pascal),
                Value = FromValue(p.GetValue(obj))
            }).ToList();

            return new ObjectInitializerExpression {
                Type = objectType,
                PropertyValues = propertyValues
            };
        }
    }
}
