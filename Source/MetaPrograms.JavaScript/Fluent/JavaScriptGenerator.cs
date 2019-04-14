using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MetaPrograms.Expressions;
using MetaPrograms.Members;

namespace MetaPrograms.JavaScript.Fluent
{
    public static class JavaScriptGenerator
    {
        public static AbstractExpression JSON(object value)
        {
            if (value == null)
            {
                return ConstantExpression.Null;
            }
            else if (value is AbstractExpression expression)
            {
                return expression;
            }
            else if (value is IList list)
            {
                return JsonOfArray(list);
            }
            else if (value is IDictionary dictionary)
            {
                return JsonOfDictionary(dictionary);
            }
            else if (!IsScalar(value))
            {
                return JsonOfObject(value);
            }
            else
            {
                return new ConstantExpression {
                    Value = value
                };
            }

            bool IsScalar(object obj)
            {
                var clrType = obj.GetType();
                return (clrType.IsPrimitive || clrType.IsEnum || obj is string);
            }
            
            AbstractExpression JsonOfArray(IList list)
            {
                var arrayItems = list
                    .Cast<object>()
                    .Select(x => JSON(x))
                    .ToList();

                return new NewArrayExpression {
                    Length = AbstractExpression.FromValue(list.Count),
                    DimensionInitializerValues = new List<List<AbstractExpression>> {arrayItems}
                };
            }
            
            AbstractExpression JsonOfDictionary(IDictionary dictionary)
            {
                return new ObjectInitializerExpression {
                    PropertyValues = dictionary.Keys
                        .Cast<object>()
                        .Select(key => new NamedPropertyValue {
                            Name = key.ToString(),
                            Value = AbstractExpression.FromValue(dictionary[key])
                        }).ToList()
                };
            }

            AbstractExpression JsonOfObject(object obj)
            {
                var properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                var propertyValues = properties.Select(p => new NamedPropertyValue {
                    Name = new IdentifierName(p.Name, language: null, style: CasingStyle.Pascal),
                    Value = JSON(p.GetValue(obj))
                }).ToList();

                return new ObjectInitializerExpression {
                    PropertyValues = propertyValues
                };
            }
        }
    }
}