using System;
using System.Collections.Immutable;

namespace MetaPrograms.CodeModel.Imperative.Members
{
    public class ProxyTypeMember : TypeMember
    {
        private TypeMember _backingType;
        private bool _backingTypeReAssigned;

        public ProxyTypeMember(TypeMemberBuilder builder)
            : base(builder)
        {
            _backingType = builder.GetTemporaryType();
            _backingTypeReAssigned = false;
        }

        public void ReAssignBackingTypeOnce(TypeMember type)
        {
            if (_backingTypeReAssigned)
            {
                throw new InvalidOperationException("Backing type was already reassigned on this proxy.");
            }

            _backingTypeReAssigned = true;
            _backingType = type;
        }

        public override bool Equals(TypeMember other)
        {
            if (other != null)
            {
                return (other.RealType == this._backingType);
            }

            return false;
        }

        public override TypeMember MakeGenericType(params TypeMember[] typeArguments)
        {
            return _backingType.MakeGenericType(typeArguments);
        }

        public override string AssemblyName => _backingType.AssemblyName;

        public override string Namespace => _backingType.Namespace;

        public override TypeMember BaseType => _backingType.BaseType;

        public override ImmutableHashSet<TypeMember> Interfaces => _backingType.Interfaces;

        public override TypeMemberKind TypeKind => _backingType.TypeKind;

        public override bool IsAbstract => _backingType.IsAbstract;

        public override bool IsValueType => _backingType.IsValueType;

        public override bool IsCollection => _backingType.IsCollection;

        public override bool IsArray => _backingType.IsArray;

        public override bool IsNullable => _backingType.IsNullable;

        public override bool IsAwaitable => _backingType.IsAwaitable;

        public override bool IsGenericType => _backingType.IsGenericType;

        public override bool IsGenericDefinition => _backingType.IsGenericDefinition;

        public override bool IsGenericParameter => _backingType.IsGenericParameter;

        public override TypeMember GenericTypeDefinition => _backingType.GenericTypeDefinition;

        public override ImmutableList<TypeMember> GenericArguments => _backingType.GenericArguments;

        public override ImmutableList<TypeMember> GenericParameters => _backingType.GenericParameters;

        public override TypeMember UnderlyingType => _backingType.UnderlyingType;

        public override ImmutableList<AbstractMember> Members => _backingType.Members;

        public override TypeGeneratorInfo Generator => _backingType.Generator;

        public override BindingCollection Bindings => _backingType.Bindings;

        public override string Name => _backingType.Name;

        public override TypeMember DeclaringType => _backingType.DeclaringType;

        public override MemberStatus Status => _backingType.Status;

        public override MemberVisibility Visibility => _backingType.Visibility;

        public override MemberModifier Modifier => _backingType.Modifier;

        public override ImmutableList<AttributeDescription> Attributes => _backingType.Attributes;

        protected internal override bool IsProxy => true;

        protected internal override TypeMember RealType => _backingType;
    }
}