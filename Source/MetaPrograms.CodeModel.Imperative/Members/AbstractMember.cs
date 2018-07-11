using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using CommonExtensions;

namespace MetaPrograms.CodeModel.Imperative.Members
{
    public abstract class AbstractMember
    {
        public virtual void AcceptVisitor(MemberVisitor visitor)
        {
            if (this.Attributes != null)
            {
                foreach (var attribute in this.Attributes)
                {
                    visitor.VisitAttribute(attribute);
                }
            }
        }

        public override string ToString()
        {
            return $"{this.GetType().Name.TrimSuffix("Member")} {this.Name}";
        }

        public BindingCollection Bindings { get; } = new BindingCollection();

        public virtual string Name { get; set; }
        public virtual ModuleMember DeclaringModule { get; set; }
        public virtual TypeMember DeclaringType { get; set; }
        public virtual MemberStatus Status { get; set; }
        public virtual MemberVisibility Visibility { get; set; }
        public virtual MemberModifier Modifier { get; set; }
        public virtual List<AttributeDescription> Attributes { get; set; } = new List<AttributeDescription>();
        public virtual bool IsTopLevel => (DeclaringType == null && DeclaringModule == null);
    }
}
