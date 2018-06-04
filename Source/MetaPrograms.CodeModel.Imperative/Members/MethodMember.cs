using System.Collections.Immutable;
using System.Reflection;
using MetaPrograms.CodeModel.Imperative.Statements;

namespace MetaPrograms.CodeModel.Imperative.Members
{
    public class MethodMember : MethodMemberBase
    {
        public MethodMember(
            string name,
            MemberRef<TypeMember> declaringType, 
            MemberStatus status, 
            MemberVisibility visibility, 
            MemberModifier modifier, 
            ImmutableList<AttributeDescription> attributes, 
            MethodSignature signature, 
            BlockStatement body) 
            : base(name, declaringType, status, visibility, modifier, attributes, signature, body)
        {
        }

        public MethodMember(
            MethodMember source, 
            Mutator<string>? name = null, 
            Mutator<MemberRef<TypeMember>>? declaringType = null, 
            Mutator<MemberStatus>? status = null, 
            Mutator<MemberVisibility>? visibility = null, 
            Mutator<MemberModifier>? modifier = null, 
            Mutator<ImmutableList<AttributeDescription>>? attributes = null, 
            Mutator<MethodSignature>? signature = null, 
            Mutator<BlockStatement>? body = null,
            bool shouldReplaceSource = false) 
            : base(
                source, name, declaringType, status, visibility, modifier, attributes, signature, body, shouldReplaceSource)
        {
        }

        public new MemberRef<MethodMember> GetRef() => new MemberRef<MethodMember>(SelfReference);

        public override AbstractMember WithAttributes(ImmutableList<AttributeDescription> attributes, bool shouldReplaceSource = false)
        {
            return new MethodMember(
                source: this,
                attributes: attributes,
                shouldReplaceSource: shouldReplaceSource);
        }

        public override MethodMemberBase WithSignature(MethodSignature signature, bool shouldReplaceSource = false)
        {
            return new MethodMember(
                source: this,
                signature: signature,
                shouldReplaceSource: shouldReplaceSource);
        }

        public override MethodMemberBase WithBody(BlockStatement block, bool shouldReplaceSource = false)
        {
            return new MethodMember(
                source: this,
                body: block,
                shouldReplaceSource: shouldReplaceSource);
        }

        public override void AcceptVisitor(MemberVisitor visitor)
        {
            base.AcceptVisitor(visitor);
            visitor.VisitMethod(this);
        }
    }
}
