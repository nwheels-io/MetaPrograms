using System.Xml.Linq;
using MetaPrograms.Expressions;
using MetaPrograms.Fluent;

namespace MetaPrograms.Adapters.JavaScript.Writer
{
    public class JsxExpressionAttribute : XAttribute
    {
        public AbstractExpression Expression { get; }

        public JsxExpressionAttribute(XName name, AbstractExpression expression)
            : base(name, "")
        {
            Expression = BlockContext.Pop(expression);
        }
    }
}
