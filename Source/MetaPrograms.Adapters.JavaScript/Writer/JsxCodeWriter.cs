using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using CommonExtensions;
using MetaPrograms;

namespace MetaPrograms.Adapters.JavaScript.Writer
{
    public class JsxCodeWriter
    {
        private readonly CodeTextBuilder _code;

        public JsxCodeWriter(CodeTextBuilder code)
        {
            _code = code;
        }

        public void Write(XElement element)
        {
            WriteStartElement(element);

            if (!element.IsEmpty)
            {
                _code.WriteListStart(opener: "", closer: "", separator: "", newLine: true);                

                foreach (var child in element.Nodes())
                {
                    _code.WriteListItem();

                    if (child is XElement childElement)
                    {
                        Write(childElement);
                    }
                    else
                    {
                        _code.Write(child.ToString());
                    }
                }
                
                _code.WriteListEnd();
            }

            WriteEndElement(element);
        }

        private void WriteStartElement(XElement element)
        {
            _code.Write($"<{element.Name}");

            foreach (var attribute in element.Attributes())
            {
                _code.Write(" ");
                
                if (attribute is JsxExpressionAttribute expressionAttribute)
                {
                    _code.Write(attribute.Name.ToString());
                    _code.Write("={");
                    JavaScriptExpressionWriter.WriteExpression(_code, expressionAttribute.Expression);
                    _code.Write("}");
                }
                else
                {
                    _code.Write(attribute.ToString());
                }
            }
            
            _code.Write(element.IsEmpty ? " />" : ">");
        }

        private void WriteEndElement(XElement element)
        {
            if (!element.IsEmpty)
            {
                _code.Write($"</{element.Name}>");
            }
        }
    }
}