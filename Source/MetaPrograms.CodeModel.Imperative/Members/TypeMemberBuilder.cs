using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace MetaPrograms.CodeModel.Imperative.Members
{
    public class TypeMemberBuilder
    {
        private readonly TypeMemberProxy _temporaryProxy;
        private readonly MemberRefState _memberSelfReference;

        public TypeMemberBuilder()
            : this(selfReference: null)
        {
        }

        public TypeMemberBuilder(MemberRefState selfReference)
        {
            this.Attributes = new List<AttributeDescription>();
            this.Interfaces = new HashSet<MemberRef<TypeMember>>();
            this.GenericArguments = new List<MemberRef<TypeMember>>();
            this.GenericParameters = new List<MemberRef<TypeMember>>();
            this.Members = new List<MemberRef<AbstractMember>>();
            this.Status = MemberStatus.Incomplete;

            _temporaryProxy = new TypeMemberProxy(this, selfReference);
            _memberSelfReference = _temporaryProxy.GetSelfReference();
        }

        public string Name { get; set; }
        public MemberRef<TypeMember> DeclaringType { get; set; }
        public MemberStatus Status { get; set; }
        public MemberVisibility Visibility { get; set; }
        public MemberModifier Modifier { get; set; }
        public List<AttributeDescription> Attributes { get; }
        public string AssemblyName { get; set; }
        public string Namespace { get; set; }
        public MemberRef<TypeMember> BaseType { get; set; }
        public HashSet<MemberRef<TypeMember>> Interfaces { get; }
        public TypeMemberKind TypeKind { get; set; }
        public bool IsAbstract { get; set; }
        public bool IsValueType { get; set; }
        public bool IsCollection { get; set; }
        public bool IsArray { get; set; }
        public bool IsNullable { get; set; }
        public bool IsAwaitable { get; set; }
        public bool IsGenericType { get; set; }
        public bool IsGenericDefinition { get; set; }
        public bool IsGenericParameter { get; set; }
        public MemberRef<TypeMember> GenericDefinition { get; set; }
        public List<MemberRef<TypeMember>> GenericArguments { get; }
        public List<MemberRef<TypeMember>> GenericParameters { get; }
        public MemberRef<TypeMember> UnderlyingType { get; set; }
        public List<MemberRef<AbstractMember>> Members { get; }
        public TypeGeneratorInfo Generator { get; set; }

        public BindingCollection Bindings => _temporaryProxy.Bindings;
        public TypeMember GetTemporaryProxy() => _temporaryProxy;
        public MemberRefState GetMemberSelfReference() => _memberSelfReference;
        public MemberRef<TypeMember> GetRef() => new MemberRef<TypeMember>(_memberSelfReference);

        private class TypeMemberProxy : TypeMember
        {
            private readonly TypeMemberBuilder _builder;

            public TypeMemberProxy(TypeMemberBuilder builder, MemberRefState selfReference = null)
                : base(builder, selfReference)
            {
                _builder = builder;
            }

            public MemberRefState GetSelfReference()
            {
                return SelfReference;
            }

            public override bool Equals(TypeMember other)
            {
                throw new NotSupportedException("TypeMemberProxy does not support Equals");
            }

            public override TypeMember MakeGenericType(params TypeMember[] typeArguments)
            {
                throw new NotSupportedException("TypeMemberProxy does not support MakeGenericType");
            }

            public override string AssemblyName => _builder.AssemblyName;

            public override string Namespace => _builder.Namespace;

            public override MemberRef<TypeMember> BaseType => _builder.BaseType;

            public override ImmutableHashSet<MemberRef<TypeMember>> Interfaces => _builder.Interfaces.ToImmutableHashSet();

            public override TypeMemberKind TypeKind => _builder.TypeKind;

            public override bool IsAbstract => _builder.IsAbstract;

            public override bool IsValueType => _builder.IsValueType;

            public override bool IsCollection => _builder.IsCollection;

            public override bool IsArray => _builder.IsArray;

            public override bool IsNullable => _builder.IsNullable;

            public override bool IsAwaitable => _builder.IsAwaitable;

            public override bool IsGenericType => _builder.IsGenericType;

            public override bool IsGenericDefinition => _builder.IsGenericDefinition;

            public override bool IsGenericParameter => _builder.IsGenericParameter;

            public override MemberRef<TypeMember> GenericTypeDefinition => _builder.GenericDefinition;

            public override ImmutableList<MemberRef<TypeMember>> GenericArguments => _builder.GenericArguments.ToImmutableList();

            public override ImmutableList<MemberRef<TypeMember>> GenericParameters => _builder.GenericParameters.ToImmutableList();

            public override MemberRef<TypeMember> UnderlyingType => _builder.UnderlyingType;

            public override ImmutableList<MemberRef<AbstractMember>> Members => _builder.Members.ToImmutableList();

            public override TypeGeneratorInfo Generator => _builder.Generator;

            public override AbstractMember WithAttributes(ImmutableList<AttributeDescription> attributes, bool shouldReplaceSource = false)
            {
                if (!shouldReplaceSource)
                {
                    throw new NotSupportedException("Parameter shouldReplaceSource must be true because TypeMemberProxy cannot create copies");
                }
                
                _builder.Attributes.Clear();
                _builder.Attributes.AddRange(attributes);
                
                return this;
            }

            public override string Name => _builder.Name;

            public override MemberRef<TypeMember> DeclaringType => _builder.DeclaringType;

            public override MemberStatus Status => _builder.Status;

            public override MemberVisibility Visibility => _builder.Visibility;

            public override MemberModifier Modifier => _builder.Modifier;

            public override ImmutableList<AttributeDescription> Attributes => _builder.Attributes.ToImmutableList();
        }
    }
}
