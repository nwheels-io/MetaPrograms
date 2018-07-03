using System.Collections.Generic;
using System.Collections.Immutable;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Expressions
{
    public class ObjectInitializerExpression : AbstractExpression
    {
        public ObjectInitializerExpression(
            IEnumerable<NamedPropertyValue> propertyValues) 
            : base(MemberRef<TypeMember>.Null)
        {
            PropertyValues = propertyValues.ToImmutableList();
        }

        public ObjectInitializerExpression(
            ObjectInitializerExpression source,
            Mutator<ImmutableList<NamedPropertyValue>>? propertyValues = null) 
            : base(source, null)
        {
            PropertyValues = propertyValues.MutatedOrOriginal(source.PropertyValues);
        }

        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitObjectInitializerExpression(this);
            PropertyValues.ForEach(nvp => nvp.Value.AcceptVisitor(visitor));
        }

        public ImmutableList<NamedPropertyValue> PropertyValues { get; }
    }
}
