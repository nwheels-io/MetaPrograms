using System.Collections.Generic;
using System.Collections.Immutable;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Expressions
{
    public class ObjectInitializerExpression : AbstractExpression
    {
        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitObjectInitializerExpression(this);
            PropertyValues.ForEach(nvp => nvp.Value.AcceptVisitor(visitor));
        }

        public List<NamedPropertyValue> PropertyValues { get; set; } = new List<NamedPropertyValue>();
    }
}
