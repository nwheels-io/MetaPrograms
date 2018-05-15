using System;
using System.Collections.Immutable;

namespace MetaPrograms.CodeModel.Imperative.Members
{
    public class TypeMemberProxy : TypeMember
    {
        private TypeMember _realType = null;

        public TypeMemberProxy(TypeMemberBuilder builder)
            : base(builder)
        {
        }

        public void AssignRealTypeOnce(TypeMember type)
        {
            if (_realType != null)
            {
                throw new InvalidOperationException("This proxy was already assigned a real type member.");
            }
            _realType = type;
        }

        public override bool IsProxy => true;

        public override TypeMember RealType => _realType;

        public override string AssemblyName => _realType.AssemblyName;

        public override string Namespace => _realType.Namespace;

        public override TypeMember BaseType => _realType.BaseType;

        public override ImmutableHashSet<TypeMember> Interfaces => _realType.Interfaces;

        public override TypeMemberKind TypeKind => _realType.TypeKind;

        public override bool IsAbstract => _realType.IsAbstract;

        public override bool IsValueType => _realType.IsValueType;

        public override bool IsCollection => _realType.IsCollection;

        public override bool IsArray => _realType.IsArray;

        public override bool IsNullable => _realType.IsNullable;

        public override bool IsAwaitable => _realType.IsAwaitable;

        public override bool IsGenericType => _realType.IsGenericType;

        public override bool IsGenericDefinition => _realType.IsGenericDefinition;

        public override bool IsGenericParameter => _realType.IsGenericParameter;

        public override TypeMember GenericTypeDefinition => _realType.GenericTypeDefinition;

        public override ImmutableList<TypeMember> GenericArguments => _realType.GenericArguments;

        public override ImmutableList<TypeMember> GenericParameters => _realType.GenericParameters;

        public override TypeMember UnderlyingType => _realType.UnderlyingType;

        public override ImmutableList<AbstractMember> Members => _realType.Members;

        public override TypeGeneratorInfo Generator => _realType.Generator;

        public override string FullName => _realType.FullName;

        public override BindingCollection Bindings => _realType.Bindings;

        public override string Name => _realType.Name;

        public override TypeMember DeclaringType => _realType.DeclaringType;

        public override MemberStatus Status => _realType.Status;

        public override MemberVisibility Visibility => _realType.Visibility;

        public override MemberModifier Modifier => _realType.Modifier;

        public override ImmutableList<AttributeDescription> Attributes => _realType.Attributes;

        private TypeMember GetRealTypeOrThrow()
        {
            if (_realType != null)
            {
                return _realType;
            }
            
            throw new InvalidOperationException("This proxy was not assigned a real type member.");
        }
    }
}