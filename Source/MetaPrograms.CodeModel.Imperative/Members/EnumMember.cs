using System.Collections.Immutable;

namespace MetaPrograms.CodeModel.Imperative.Members
{
    public class EnumMember : AbstractMember
    {
        public EnumMember(
            string name, 
            TypeMember declaringType, 
            MemberStatus status, 
            MemberVisibility visibility, 
            MemberModifier modifier, 
            ImmutableList<AttributeDescription> attributes, 
            object value) 
            : base(name, declaringType, status, visibility, modifier, attributes)
        {
            Value = value;
        }

        public EnumMember(
            EnumMember source, 
            Mutator<string>? name, 
            Mutator<TypeMember>? declaringType, 
            Mutator<MemberStatus>? status, 
            Mutator<MemberVisibility>? visibility, 
            Mutator<MemberModifier>? modifier, 
            Mutator<ImmutableList<AttributeDescription>>? attributes,
            Mutator<object>? value = null) : base(source, name, declaringType, status, visibility, modifier, attributes)
        {
            Value = value.MutatedOrOriginal(source.Value);
        }

        public override void AcceptVisitor(MemberVisitor visitor)
        {
            base.AcceptVisitor(visitor);
            visitor.VisitEnumMember(this);
        }

        public object Value { get; }
    }
}
