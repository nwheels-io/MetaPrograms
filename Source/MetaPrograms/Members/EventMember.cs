using System.Collections.Immutable;
using System.Reflection;

namespace MetaPrograms.Members
{
    public class EventMember : AbstractMember
    {
        public override void AcceptVisitor(MemberVisitor visitor)
        {
            base.AcceptVisitor(visitor);

            visitor.VisitEvent(this);

            if (Adder != null)
            {
                Adder.AcceptVisitor(visitor);
            }

            if (Remover != null)
            {
                Remover.AcceptVisitor(visitor);
            }
        }

        public TypeMember DelegateType { get; set; }
        public MethodMember Adder { get; set; }
        public MethodMember Remover { get; set; }
    }
}
