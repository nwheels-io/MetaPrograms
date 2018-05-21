using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace MetaPrograms.CodeModel.Imperative.Members
{
    public abstract class AbstractMember
    {
        protected AbstractMember(
            string name,
            MemberRef<TypeMember> declaringType,
            MemberStatus status,
            MemberVisibility visibility,
            MemberModifier modifier,
            ImmutableList<AttributeDescription> attributes,
            MemberRefState selfReference = null)
        {
            this.SelfReference = selfReference ?? new MemberRefState(this);

            Name = name;
            DeclaringType = declaringType;
            Status = status;
            Visibility = visibility;
            Modifier = modifier;
            Attributes = attributes;
        }

        protected AbstractMember(
            AbstractMember source,
            Mutator<string>? name = null,
            Mutator<MemberRef<TypeMember>>? declaringType = null,
            Mutator<MemberStatus>? status = null,
            Mutator<MemberVisibility>? visibility = null,
            Mutator<MemberModifier>? modifier = null,
            Mutator<ImmutableList<AttributeDescription>>? attributes = null,
            bool preserveMemberRef = false)
        {
            Name = name.MutatedOrOriginal(source.Name);
            DeclaringType = declaringType.MutatedOrOriginal(source.DeclaringType);
            Status = status.MutatedOrOriginal(source.Status);
            Visibility = visibility.MutatedOrOriginal(source.Visibility);
            Modifier = modifier.MutatedOrOriginal(source.Modifier);
            Attributes = attributes.MutatedOrOriginal(source.Attributes);
            
            if (preserveMemberRef)
            {
                this.SelfReference = source.SelfReference;
                this.SelfReference.Reassign(this);
            }
            else
            {
                this.SelfReference = new MemberRefState(this);
            }
        }

        //public bool HasAttribute<TAttribute>()
        //    where TAttribute : Attribute
        //{
        //    return TryGetAttribute<TAttribute>(out TAttribute attribute);
        //}

        //public bool TryGetAttribute<TAttribute>(out TAttribute attribute)
        //    where TAttribute : Attribute
        //{
        //    var description = this.Attributes.FirstOrDefault(a => a.Binding is TAttribute);
        //    attribute = (description?.Binding as TAttribute);
        //    return (attribute != null);
        //}

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

        public virtual BindingCollection Bindings { get; } = new BindingCollection();

        public virtual string Name { get; }
        public virtual MemberRef<TypeMember> DeclaringType { get; }
        public virtual MemberStatus Status { get; }
        public virtual MemberVisibility Visibility { get; }
        public virtual MemberModifier Modifier { get; }
        public virtual ImmutableList<AttributeDescription> Attributes { get; }

        protected MemberRefState SelfReference { get; }
    }
}
