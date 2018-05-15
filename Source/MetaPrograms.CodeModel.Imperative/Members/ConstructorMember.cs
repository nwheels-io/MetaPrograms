﻿using System.Collections.Immutable;
using System.Reflection;
using MetaPrograms.CodeModel.Imperative.Expressions;
using MetaPrograms.CodeModel.Imperative.Statements;

namespace MetaPrograms.CodeModel.Imperative.Members
{
    public class ConstructorMember : MethodMemberBase
    {
        public ConstructorMember(
            string name,
            MemberRef<TypeMember> declaringType, 
            MemberStatus status, 
            MemberVisibility visibility, 
            MemberModifier modifier, 
            ImmutableList<AttributeDescription> attributes, 
            MethodSignature signature, 
            BlockStatement body, 
            MethodCallExpression callThisConstructor, 
            MethodCallExpression callBaseConstructor) 
            : base(name, declaringType, status, visibility, modifier, attributes, signature, body)
        {
            CallThisConstructor = callThisConstructor;
            CallBaseConstructor = callBaseConstructor;
        }

        public ConstructorMember(
            ConstructorMember source,
            Mutator<string>? name = null, 
            Mutator<MemberRef<TypeMember>>? declaringType = null, 
            Mutator<MemberStatus>? status = null, 
            Mutator<MemberVisibility>? visibility = null, 
            Mutator<MemberModifier>? modifier = null, 
            Mutator<ImmutableList<AttributeDescription>>? attributes = null, 
            Mutator<MethodSignature>? signature = null, 
            Mutator<BlockStatement>? body = null,
            Mutator<MethodCallExpression>? callThisConstructor = null,
            Mutator<MethodCallExpression>? callBaseConstructor = null) 
            : base(source, name, declaringType, status, visibility, modifier, attributes, signature, body)
        {
            CallThisConstructor = callThisConstructor.MutatedOrOriginal(source.CallThisConstructor);
            CallBaseConstructor = callBaseConstructor.MutatedOrOriginal(source.CallBaseConstructor);
        }

        public new MemberRef<ConstructorMember> GetRef() => new MemberRef<ConstructorMember>(SelfReference);

        public override void AcceptVisitor(MemberVisitor visitor)
        {
            base.AcceptVisitor(visitor);
            visitor.VisitConstructor(this);
        }

        public MethodCallExpression CallThisConstructor { get; }
        public MethodCallExpression CallBaseConstructor { get; }
    }
}
