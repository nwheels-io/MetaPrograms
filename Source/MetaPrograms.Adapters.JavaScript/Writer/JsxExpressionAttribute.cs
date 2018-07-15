using System.Xml.Linq;
using MetaPrograms.CodeModel.Imperative.Expressions;

namespace MetaPrograms.Adapters.JavaScript.Writer
{
    public class JsxExpressionAttribute : XAttribute
    {
        public AbstractExpression Expression { get; }

        public JsxExpressionAttribute(XName name, AbstractExpression expression)
            : base(name, "")
        {
            Expression = expression;
        }
    }
}
