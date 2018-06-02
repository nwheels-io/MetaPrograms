using System.Collections.Immutable;
using System.Reflection;
using MetaPrograms.CodeModel.Imperative.Expressions;
using MetaPrograms.CodeModel.Imperative.Statements;

namespace MetaPrograms.CodeModel.Imperative.Members
{
    public class ConstructorMember : MethodMemberBase
    {
        public ConstructorMember(
            MemberRef<TypeMember> declaringType, 
            MemberStatus status, 
            MemberVisibility visibility, 
            MemberModifier modifier, 
            ImmutableList<AttributeDescription> attributes, 
            MethodSignature signature, 
            BlockStatement body, 
            MethodCallExpression callThisConstructor, 
            MethodCallExpression callBaseConstructor) 
            : base(name: "constructor", declaringType, status, visibility, modifier, attributes, signature, body)
        {
            CallThisConstructor = callThisConstructor;
            CallBaseConstructor = callBaseConstructor;
        }

        public ConstructorMember(
            ConstructorMember source,
            Mutator<MemberRef<TypeMember>>? declaringType = null, 
            Mutator<MemberStatus>? status = null, 
            Mutator<MemberVisibility>? visibility = null, 
            Mutator<MemberModifier>? modifier = null, 
            Mutator<ImmutableList<AttributeDescription>>? attributes = null, 
            Mutator<MethodSignature>? signature = null, 
            Mutator<BlockStatement>? body = null,
            Mutator<MethodCallExpression>? callThisConstructor = null,
            Mutator<MethodCallExpression>? callBaseConstructor = null,
            bool shouldReplaceSource = false) 
            : base(source, name: null, declaringType, status, visibility, modifier, attributes, signature, body, shouldReplaceSource)
        {
            CallThisConstructor = callThisConstructor.MutatedOrOriginal(source.CallThisConstructor);
            CallBaseConstructor = callBaseConstructor.MutatedOrOriginal(source.CallBaseConstructor);
        }

        public new MemberRef<ConstructorMember> GetRef() => new MemberRef<ConstructorMember>(SelfReference);

        public override AbstractMember WithAttributes(ImmutableList<AttributeDescription> attributes, bool shouldReplaceSource = false)
        {
            return new ConstructorMember(
                source: this, 
                attributes: attributes, 
                shouldReplaceSource: shouldReplaceSource);
        }
        
        public override MethodMemberBase WithSignature(MethodSignature signature, bool shouldReplaceSource = false)
        {
            return new ConstructorMember(
                source: this,
                signature: signature,
                shouldReplaceSource: shouldReplaceSource);
        }


        public override void AcceptVisitor(MemberVisitor visitor)
        {
            base.AcceptVisitor(visitor);
            visitor.VisitConstructor(this);
        }

        public MethodCallExpression CallThisConstructor { get; }
        public MethodCallExpression CallBaseConstructor { get; }
    }
}
