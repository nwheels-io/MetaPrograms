using System.Collections.Immutable;
using System.Reflection;
using MetaPrograms.CodeModel.Imperative.Expressions;
using MetaPrograms.CodeModel.Imperative.Statements;

namespace MetaPrograms.CodeModel.Imperative.Members
{
    public class ConstructorMember : MethodMemberBase
    {
        public ConstructorMember(
            string name, 
            TypeMember declaringType, 
            MemberStatus status, 
            MemberVisibility visibility, 
            MemberModifier modifier, 
            ImmutableList<AttributeDescription> attributes, 
            MethodSignature signature, 
            BlockStatement body, 
            MethodCallExpression callThisConstructor, 
            MethodCallExpression callBaseConstructor, 
            ConstructorInfo clrBinding) 
            : base(name, declaringType, status, visibility, modifier, attributes, signature, body)
        {
            CallThisConstructor = callThisConstructor;
            CallBaseConstructor = callBaseConstructor;
            ClrBinding = clrBinding;
        }

        public ConstructorMember(
            ConstructorMember source,
            Mutator<string>? name = null, 
            Mutator<TypeMember>? declaringType = null, 
            Mutator<MemberStatus>? status = null, 
            Mutator<MemberVisibility>? visibility = null, 
            Mutator<MemberModifier>? modifier = null, 
            Mutator<ImmutableList<AttributeDescription>>? attributes = null, 
            Mutator<MethodSignature>? signature = null, 
            Mutator<BlockStatement>? body = null,
            Mutator<MethodCallExpression>? callThisConstructor = null,
            Mutator<MethodCallExpression>? callBaseConstructor = null,
            Mutator<ConstructorInfo>? clrBinding = null) 
            : base(source, name, declaringType, status, visibility, modifier, attributes, signature, body)
        {
            CallThisConstructor = callThisConstructor.MutatedOrOriginal(source.CallThisConstructor);
            CallBaseConstructor = callBaseConstructor.MutatedOrOriginal(source.CallBaseConstructor);
            ClrBinding = clrBinding.MutatedOrOriginal(source.ClrBinding);
        }

        public override void AcceptVisitor(MemberVisitor visitor)
        {
            base.AcceptVisitor(visitor);
            visitor.VisitConstructor(this);
        }

        public MethodCallExpression CallThisConstructor { get; }
        public MethodCallExpression CallBaseConstructor { get; }
        public ConstructorInfo ClrBinding { get; }
    }
}
