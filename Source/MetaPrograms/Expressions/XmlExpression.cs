using System.Xml.Linq;

namespace MetaPrograms.Expressions
{
    public class XmlExpression : AbstractExpression
    {
        public XElement Xml { get; set; }
        
        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitXmlExpression(this);
        }

        public override AbstractExpression AcceptRewriter(StatementRewriter rewriter)
        {
            return rewriter.RewriteXmlExpression(this);
        }
    }
}