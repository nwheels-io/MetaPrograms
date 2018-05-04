using System.Collections.Immutable;
using System.Reflection;

namespace MetaPrograms.CodeModel.Imperative.Members
{
    public class PropertyMember : AbstractMember
    {
        public PropertyMember(
            string name, 
            TypeMember declaringType, 
            MemberStatus status, 
            MemberVisibility visibility, 
            MemberModifier modifier, 
            ImmutableList<AttributeDescription> attributes, 
            TypeMember propertyType, 
            MethodMember getter, 
            MethodMember setter) 
            : base(name, declaringType, status, visibility, modifier, attributes)
        {
            PropertyType = propertyType;
            Getter = getter;
            Setter = setter;
        }

        public PropertyMember(
            PropertyMember source,
            Mutator<TypeMember>? propertyType,
            Mutator<MethodMember>? getter,
            Mutator<MethodMember>? setter, 
            Mutator<string>? name = null, 
            Mutator<TypeMember>? declaringType = null, 
            Mutator<MemberStatus>? status = null, 
            Mutator<MemberVisibility>? visibility = null, 
            Mutator<MemberModifier>? modifier = null, 
            Mutator<ImmutableList<AttributeDescription>>? attributes = null) 
            : base(source, name, declaringType, status, visibility, modifier, attributes)
        {
            PropertyType = propertyType.MutatedOrOriginal(source.PropertyType);
            Getter = getter.MutatedOrOriginal(source.Getter);
            Setter = setter.MutatedOrOriginal(source.Setter);
        }

        public override void AcceptVisitor(MemberVisitor visitor)
        {
            base.AcceptVisitor(visitor);

            visitor.VisitProperty(this);

            if (this.Getter != null)
            {
                this.Getter.AcceptVisitor(visitor);
            }

            if (this.Setter != null)
            {
                this.Setter.AcceptVisitor(visitor);
            }
        }

        public TypeMember PropertyType { get; }
        public MethodMember Getter { get; }
        public MethodMember Setter { get; }
    }
}
