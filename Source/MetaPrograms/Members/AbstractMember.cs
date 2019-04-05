using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using MetaPrograms.Extensions;

namespace MetaPrograms.Members
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

        public T GetBindingOrThrow<T>() where T : class
        {
            var binding = TryGetBinding<T>();
            
            if (binding != null)
            {
                return binding;
            }
            
            throw new KeyNotFoundException($"Could not find binding type '{typeof(T).Name}' on member '${Name}'.");
        }

        public T TryGetBinding<T>() where T : class
        {
            return Bindings.OfType<T>().FirstOrDefault();
        }

        public AttributeDescription TryGetAttribute(TypeMember attributeType)
        {
            return Attributes.FirstOrDefault(attr => attr.AttributeType == attributeType);
        }
        
        public override string ToString()
        {
            return $"{this.GetType().Name.TrimSuffix("Member")} {this.Name}";
        }

        public BindingCollection Bindings { get; } = new BindingCollection();
        public virtual IdentifierName Name { get; set; }
        public virtual ModuleMember DeclaringModule { get; set; }
        public virtual TypeMember DeclaringType { get; set; }
        public virtual MemberStatus Status { get; set; }
        public virtual MemberVisibility Visibility { get; set; }
        public virtual MemberModifier Modifier { get; set; }
        public virtual List<AttributeDescription> Attributes { get; set; } = new List<AttributeDescription>();
        public virtual bool IsDefaultExport { get; set; }
        public virtual bool IsTopLevel => (DeclaringType == null && DeclaringModule == null);
    }
}
