using System.Collections.Immutable;
using System.Reflection;
using MetaPrograms.CodeModel.Imperative.Statements;

namespace MetaPrograms.CodeModel.Imperative.Members
{
    public class MethodMember : MethodMemberBase
    {
        public MethodMember(
            string name, 
            TypeMember declaringType, 
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
            Mutator<TypeMember>? declaringType = null, 
            Mutator<MemberStatus>? status = null, 
            Mutator<MemberVisibility>? visibility = null, 
            Mutator<MemberModifier>? modifier = null, 
            Mutator<ImmutableList<AttributeDescription>>? attributes = null, 
            Mutator<MethodSignature>? signature = null, 
            Mutator<BlockStatement>? body = null) 
            : base(source, name, declaringType, status, visibility, modifier, attributes, signature, body)
        {
        }

        public override void AcceptVisitor(MemberVisitor visitor)
        {
            base.AcceptVisitor(visitor);
            visitor.VisitMethod(this);
        }
    }
}
