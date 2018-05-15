using System;
using System.Collections.Immutable;

namespace MetaPrograms.CodeModel.Imperative.Members
{
    public class ProxyTypeMember : TypeMember
    {
        private TypeMember _realType = null;

        public ProxyTypeMember(TypeMemberBuilder builder)
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

        public override bool Equals(TypeMember other)
        {
            if (other != null)
            {
                return (other.RealType == this._realType);
            }

            return false;
        }

        public override TypeMember MakeGenericType(params TypeMember[] typeArguments)
        {
            return GetRealTypeOrThrow().MakeGenericType(typeArguments);
        }

        public override string AssemblyName => GetRealTypeOrThrow().AssemblyName;

        public override string Namespace => GetRealTypeOrThrow().Namespace;

        public override TypeMember BaseType => GetRealTypeOrThrow().BaseType;

        public override ImmutableHashSet<TypeMember> Interfaces => GetRealTypeOrThrow().Interfaces;

        public override TypeMemberKind TypeKind => GetRealTypeOrThrow().TypeKind;

        public override bool IsAbstract => GetRealTypeOrThrow().IsAbstract;

        public override bool IsValueType => GetRealTypeOrThrow().IsValueType;

        public override bool IsCollection => GetRealTypeOrThrow().IsCollection;

        public override bool IsArray => GetRealTypeOrThrow().IsArray;

        public override bool IsNullable => GetRealTypeOrThrow().IsNullable;

        public override bool IsAwaitable => GetRealTypeOrThrow().IsAwaitable;

        public override bool IsGenericType => GetRealTypeOrThrow().IsGenericType;

        public override bool IsGenericDefinition => GetRealTypeOrThrow().IsGenericDefinition;

        public override bool IsGenericParameter => GetRealTypeOrThrow().IsGenericParameter;

        public override TypeMember GenericTypeDefinition => GetRealTypeOrThrow().GenericTypeDefinition;

        public override ImmutableList<TypeMember> GenericArguments => GetRealTypeOrThrow().GenericArguments;

        public override ImmutableList<TypeMember> GenericParameters => GetRealTypeOrThrow().GenericParameters;

        public override TypeMember UnderlyingType => GetRealTypeOrThrow().UnderlyingType;

        public override ImmutableList<AbstractMember> Members => GetRealTypeOrThrow().Members;

        public override TypeGeneratorInfo Generator => GetRealTypeOrThrow().Generator;

        public override BindingCollection Bindings => GetRealTypeOrThrow().Bindings;

        public override string Name => GetRealTypeOrThrow().Name;

        public override TypeMember DeclaringType => GetRealTypeOrThrow().DeclaringType;

        public override MemberStatus Status => GetRealTypeOrThrow().Status;

        public override MemberVisibility Visibility => GetRealTypeOrThrow().Visibility;

        public override MemberModifier Modifier => GetRealTypeOrThrow().Modifier;

        public override ImmutableList<AttributeDescription> Attributes => GetRealTypeOrThrow().Attributes;

        protected internal override bool IsProxy => true;

        protected internal override TypeMember RealType => _realType;

        private TypeMember GetRealTypeOrThrow()
        {
            if (_realType != null)
            {
                return _realType;
            }
            
            throw new InvalidOperationException(
                "This proxy was not assigned a real instance. " +
                "Requested operation can only be performed on a real instance. " +
                "Call this operation after project reading is done: this is when all proxies are guaranteed to have real instances.");
        }
    }
}