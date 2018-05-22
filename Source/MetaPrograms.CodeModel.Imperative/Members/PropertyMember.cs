using System.Collections.Immutable;
using System.Reflection;

namespace MetaPrograms.CodeModel.Imperative.Members
{
    public class PropertyMember : AbstractMember
    {
        public PropertyMember(
            string name,
            MemberRef<TypeMember> declaringType, 
            MemberStatus status, 
            MemberVisibility visibility, 
            MemberModifier modifier, 
            ImmutableList<AttributeDescription> attributes,
            MemberRef<TypeMember> propertyType,
            MemberRef<MethodMember> getter,
            MemberRef<MethodMember> setter) 
            : base(name, declaringType, status, visibility, modifier, attributes)
        {
            PropertyType = propertyType;
            Getter = getter;
            Setter = setter;
        }

        public PropertyMember(
            PropertyMember source,
            Mutator<MemberRef<TypeMember>>? propertyType,
            Mutator<MemberRef<MethodMember>>? getter,
            Mutator<MemberRef<MethodMember>>? setter, 
            Mutator<string>? name = null, 
            Mutator<MemberRef<TypeMember>>? declaringType = null, 
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

        public MemberRef<PropertyMember> GetRef() => new MemberRef<PropertyMember>(SelfReference);

        public override void AcceptVisitor(MemberVisitor visitor)
        {
            base.AcceptVisitor(visitor);

            visitor.VisitProperty(this);

            this.Getter.Get()?.AcceptVisitor(visitor);
            this.Setter.Get()?.AcceptVisitor(visitor);
        }

        public MemberRef<TypeMember> PropertyType { get; }
        public MemberRef<MethodMember> Getter { get; }
        public MemberRef<MethodMember> Setter { get; }
    }
}
