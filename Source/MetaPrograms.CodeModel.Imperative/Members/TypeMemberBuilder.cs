using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace MetaPrograms.CodeModel.Imperative.Members
{
    public class TypeMemberBuilder
    {
        public TypeMemberBuilder()
        {
            this.Attributes = new List<AttributeDescription>();
            this.Interfaces = new HashSet<TypeMember>();
            this.GenericArguments = new List<TypeMember>();
            this.GenericParameters = new List<TypeMember>();
            this.Members = new List<AbstractMember>();
        }

        public string Name { get; set; }
        public TypeMember DeclaringType { get; set; }
        public MemberStatus Status { get; set; }
        public MemberVisibility Visibility { get; set; }
        public MemberModifier Modifier { get; set; }
        public List<AttributeDescription> Attributes { get; }
        public string AssemblyName { get; set; }
        public string Namespace { get; set; }
        public TypeMember BaseType { get; set; }
        public HashSet<TypeMember> Interfaces { get; }
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
        public TypeMember GenericDefinition { get; set; }
        public List<TypeMember> GenericArguments { get; }
        public List<TypeMember> GenericParameters { get; }
        public TypeMember UnderlyingType { get; set; }
        public List<AbstractMember> Members { get; }
        public TypeGeneratorInfo Generator { get; set; }

        public TypeMember GetTemporaryType()
        {
            return new TemporaryTypeMember(this);
        }

        private class TemporaryTypeMember : TypeMember
        {
            private readonly TypeMemberBuilder _builder;

            public TemporaryTypeMember(TypeMemberBuilder builder)
                : base(builder)
            {
                _builder = builder;
            }

            public override bool Equals(TypeMember other)
            {
                throw new NotSupportedException("TemporaryTypeMember does not support Equals");
            }

            public override TypeMember MakeGenericType(params TypeMember[] typeArguments)
            {
                throw new NotSupportedException("TemporaryTypeMember does not support MakeGenericType");
            }

            public override string AssemblyName => _builder.AssemblyName;

            public override string Namespace => _builder.Namespace;

            public override TypeMember BaseType => _builder.BaseType;

            public override ImmutableHashSet<TypeMember> Interfaces => _builder.Interfaces.ToImmutableHashSet();

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

            public override TypeMember GenericTypeDefinition => _builder.GenericDefinition;

            public override ImmutableList<TypeMember> GenericArguments => _builder.GenericArguments.ToImmutableList();

            public override ImmutableList<TypeMember> GenericParameters => _builder.GenericParameters.ToImmutableList();

            public override TypeMember UnderlyingType => _builder.UnderlyingType;

            public override ImmutableList<AbstractMember> Members => _builder.Members.ToImmutableList();

            public override TypeGeneratorInfo Generator => _builder.Generator;

            public override string Name => _builder.Name;

            public override TypeMember DeclaringType => _builder.DeclaringType;

            public override MemberStatus Status => _builder.Status;

            public override MemberVisibility Visibility => _builder.Visibility;

            public override MemberModifier Modifier => _builder.Modifier;

            public override ImmutableList<AttributeDescription> Attributes => _builder.Attributes.ToImmutableList();

            protected internal override bool IsProxy => throw new NotSupportedException("TemporaryTypeMember does not support IsProxy");

            protected internal override TypeMember RealType => throw new NotSupportedException("TemporaryTypeMember does not support RealType");

        }
    }
}
