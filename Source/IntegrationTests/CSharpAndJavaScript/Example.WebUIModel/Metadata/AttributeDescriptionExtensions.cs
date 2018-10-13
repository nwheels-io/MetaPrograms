using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MetaPrograms.Expressions;
using MetaPrograms.Members;

namespace Example.WebUIModel.Metadata
{
    public static class AttributeDescriptionExtensions
    {
        public static bool TryGetPropertyValue<T>(this AttributeDescription attribute, string propertyName, out T value)
        {
            var propertyValue = attribute.PropertyValues.FirstOrDefault(pair => pair.Name == propertyName);

            if (propertyValue != null && propertyValue.Value is ConstantExpression constant && constant.Value is T typedValue)
            {
                value = typedValue;
                return true;
            }

            value = default;
            return false;
        }

        public static T GetPropertyValueOrDefault<T>(this AttributeDescription attribute, string propertyName, T defaultValue = default(T))
        {
            if (TryGetPropertyValue<T>(attribute, propertyName, out var value))
            {
                return value;
            }

            return defaultValue;
        }

        public static T GetConstructorArgumentOrDefault<T>(this AttributeDescription attribute, int index = 0, T defaultValue = default(T))
        {
            if (index >= 0 && index < attribute.ConstructorArguments.Count)
            {
                if (attribute.ConstructorArguments[index] is ConstantExpression constant)
                {
                    if (constant.Value is T value)
                    {
                        return value;
                    }
                }
            }

            return defaultValue;
        }
    }
}
