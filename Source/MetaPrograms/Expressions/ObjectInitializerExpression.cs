using System.Collections.Generic;
using System.Collections.Immutable;
using MetaPrograms.Members;

namespace MetaPrograms.Expressions
{
    public class ObjectInitializerExpression : AbstractExpression
    {
        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitObjectInitializerExpression(this);
            PropertyValues.ForEach(nvp => nvp.Value.AcceptVisitor(visitor));
        }

        public override AbstractExpression AcceptRewriter(StatementRewriter rewriter)
        {
            return rewriter.RewriteObjectInitializerExpression(this);
        }

        public List<NamedPropertyValue> PropertyValues { get; set; } = new List<NamedPropertyValue>();
    }
}
