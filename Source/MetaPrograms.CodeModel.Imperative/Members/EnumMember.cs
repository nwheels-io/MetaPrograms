using System.Collections.Immutable;

namespace MetaPrograms.CodeModel.Imperative.Members
{
    public class EnumMember : AbstractMember
    {
        public EnumMember(
            string name,
            MemberRef<TypeMember> declaringType, 
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
            Mutator<string>? name = null, 
            Mutator<MemberRef<TypeMember>>? declaringType = null, 
            Mutator<MemberStatus>? status = null, 
            Mutator<MemberVisibility>? visibility = null, 
            Mutator<MemberModifier>? modifier = null, 
            Mutator<ImmutableList<AttributeDescription>>? attributes = null,
            Mutator<object>? value = null,
            bool shouldReplaceSource = false) 
            : base(source, name, declaringType, status, visibility, modifier, attributes, shouldReplaceSource)
        {
            Value = value.MutatedOrOriginal(source.Value);
        }

        public MemberRef<EnumMember> GetRef() => new MemberRef<EnumMember>(SelfReference);

        public override AbstractMember WithAttributes(ImmutableList<AttributeDescription> attributes, bool shouldReplaceSource = false)
        {
            return new EnumMember(
                source: this,
                attributes: attributes,
                shouldReplaceSource: shouldReplaceSource);
        }

        public override void AcceptVisitor(MemberVisitor visitor)
        {
            base.AcceptVisitor(visitor);
            visitor.VisitEnumMember(this);
        }

        public object Value { get; }
    }
}
