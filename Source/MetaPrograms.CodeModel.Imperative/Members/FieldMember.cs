using System.Collections.Immutable;
using System.Reflection;
using MetaPrograms.CodeModel.Imperative.Expressions;

namespace MetaPrograms.CodeModel.Imperative.Members
{
    public class FieldMember : AbstractMember
    {
        public FieldMember(
            string name,
            MemberRef<TypeMember> declaringType, 
            MemberStatus status, 
            MemberVisibility visibility, 
            MemberModifier modifier, 
            ImmutableList<AttributeDescription> attributes,
            MemberRef<TypeMember> type, 
            bool isReadOnly, 
            AbstractExpression initializer) 
            : base(name, declaringType, status, visibility, modifier, attributes)
        {
            Type = type;
            IsReadOnly = isReadOnly;
            Initializer = initializer;
        }

        public FieldMember(
            FieldMember source,
            Mutator<MemberRef<TypeMember>>? type,
            Mutator<FieldInfo>? clrBinding,
            Mutator<bool>? isReadOnly,
            Mutator<AbstractExpression>? initializer, 
            Mutator<string>? name = null, 
            Mutator<MemberRef<TypeMember>>? declaringType = null, 
            Mutator<MemberStatus>? status = null, 
            Mutator<MemberVisibility>? visibility = null, 
            Mutator<MemberModifier>? modifier = null, 
            Mutator<ImmutableList<AttributeDescription>>? attributes = null) 
            : base(source, name, declaringType, status, visibility, modifier, attributes)
        {
            Type = type.MutatedOrOriginal(source.Type);
            IsReadOnly = isReadOnly.MutatedOrOriginal(source.IsReadOnly);
            Initializer = initializer.MutatedOrOriginal(source.Initializer);
        }

        public MemberRef<FieldMember> GetRef() => new MemberRef<FieldMember>(SelfReference);

        public override void AcceptVisitor(MemberVisitor visitor)
        {
            base.AcceptVisitor(visitor);
            visitor.VisitField(this);
        }

        public MemberRef<TypeMember> Type { get; }
        public bool IsReadOnly { get; }
        public AbstractExpression Initializer { get; }
    }
}
