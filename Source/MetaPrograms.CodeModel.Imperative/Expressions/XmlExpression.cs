using System.Xml.Linq;

namespace MetaPrograms.CodeModel.Imperative.Expressions
{
    public class XmlExpression : AbstractExpression
    {
        public XElement Xml { get; set; }
        
        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitXmlExpression(this);
        }
    }
}