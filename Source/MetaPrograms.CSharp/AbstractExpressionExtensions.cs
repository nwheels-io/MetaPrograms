using MetaPrograms.Expressions;
using Microsoft.CodeAnalysis;

namespace MetaPrograms.CSharp
{
    public static class AbstractExpressionExtensions
    {
        public static object GetConstantValueOrNull(this AbstractExpression expression)
        {
            if (TryGetConstantValue(expression, out var value))
            {
                return value;
            }

            return null;
        }

        public static bool TryGetConstantValue(this AbstractExpression expression, out object value)
        {
            if (expression is ConstantExpression constant)
            {
                if (constant.Value is Optional<object>) //TODO: can safely remove this case?
                {
                    var optional = (Optional<object>) constant.Value;
                    var copyOfValue = optional.HasValue ? optional.Value : null;
                    value = copyOfValue;
                    return optional.HasValue;
                }
                else
                {
                    value = constant.Value;
                    return (value != null);
                }
            }

            value = null;
            return false;
        }
    }
}
