using System.Collections.Immutable;
using System.Reflection;

namespace MetaPrograms.CodeModel.Imperative.Members
{
    public class PropertyMember : AbstractMember
    {
        public override void AcceptVisitor(MemberVisitor visitor)
        {
            base.AcceptVisitor(visitor);

            visitor.VisitProperty(this);

            this.Getter?.AcceptVisitor(visitor);
            this.Setter?.AcceptVisitor(visitor);
        }

        public TypeMember PropertyType { get; set; }
        public MethodMember Getter { get; set; }
        public MethodMember Setter { get; set; }
    }
}
