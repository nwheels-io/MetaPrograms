using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace MetaPrograms.CodeModel.Imperative.Members
{
    public class RealTypeMember : TypeMember
    {
        public RealTypeMember(TypeMemberBuilder builder)
            : base(builder)
        {
            this.AssemblyName = builder.AssemblyName;
            this.Namespace = builder.Namespace;
            this.BaseType = builder.BaseType;
            this.Interfaces = builder.Interfaces.ToImmutableHashSet();
            this.TypeKind = builder.TypeKind;
            this.IsAbstract = builder.IsAbstract;
            this.IsValueType = builder.IsValueType;
            this.IsCollection = builder.IsCollection;
            this.IsArray = builder.IsArray;
            this.IsNullable = builder.IsNullable;
            this.IsAwaitable = builder.IsAwaitable;
            this.IsGenericType = builder.IsGenericType;
            this.IsGenericDefinition = builder.IsGenericDefinition;
            this.IsGenericParameter = builder.IsGenericParameter;
            this.GenericTypeDefinition = builder.GenericTypeDefinition;
            this.GenericArguments = builder.GenericTypeArguments.ToImmutableList();
            this.GenericParameters = builder.GenericTypeParameters.ToImmutableList();
            this.UnderlyingType = builder.UnderlyingType;
            this.Members = builder.Members.ToImmutableList();
            this.Generator = builder.Generator;
        }

        public RealTypeMember(TypeMember source, TypeMemberMutator mutator)
            : base(source, mutator)
        {
            this.AssemblyName = mutator.AssemblyName.MutatedOrOriginal(source.AssemblyName);
            this.Namespace = mutator.Namespace.MutatedOrOriginal(source.AssemblyName);
            this.BaseType = mutator.BaseType.MutatedOrOriginal(source.BaseType);
            this.Interfaces = mutator.Interfaces.MutatedOrOriginal(source.Interfaces);
            this.TypeKind = mutator.TypeKind.MutatedOrOriginal(source.TypeKind);
            this.IsAbstract = mutator.IsAbstract.MutatedOrOriginal(source.IsAbstract);
            this.IsValueType = mutator.IsValueType.MutatedOrOriginal(source.IsValueType);
            this.IsCollection = mutator.IsCollection.MutatedOrOriginal(source.IsCollection);
            this.IsArray = mutator.IsArray.MutatedOrOriginal(source.IsArray);
            this.IsNullable = mutator.IsNullable.MutatedOrOriginal(source.IsNullable);
            this.IsAwaitable = mutator.IsAwaitable.MutatedOrOriginal(source.IsAwaitable);
            this.IsGenericType = mutator.IsGenericType.MutatedOrOriginal(source.IsGenericType);
            this.IsGenericDefinition = mutator.IsGenericDefinition.MutatedOrOriginal(source.IsGenericDefinition);
            this.IsGenericParameter = mutator.IsGenericParameter.MutatedOrOriginal(source.IsGenericParameter);
            this.GenericTypeDefinition = mutator.GenericTypeDefinition.MutatedOrOriginal(source.GenericTypeDefinition);
            this.GenericArguments = mutator.GenericArguments.MutatedOrOriginal(source.GenericArguments);
            this.GenericParameters = mutator.GenericParameters.MutatedOrOriginal(source.GenericParameters);
            this.UnderlyingType = mutator.UnderlyingType.MutatedOrOriginal(source.UnderlyingType);
            this.Members = mutator.Members.MutatedOrOriginal(source.Members);
            this.Generator = mutator.Generator.MutatedOrOriginal(source.Generator);
        }

        public override string AssemblyName { get; }
        public override string Namespace { get; }
        public override TypeMember BaseType { get; }
        public override ImmutableHashSet<TypeMember> Interfaces { get; }
        public override TypeMemberKind TypeKind { get; }
        public override bool IsAbstract { get; }
        public override bool IsValueType { get; }
        public override bool IsCollection { get; }
        public override bool IsArray { get; }
        public override bool IsNullable { get; }
        public override bool IsAwaitable { get; }
        public override bool IsGenericType { get; }
        public override bool IsGenericDefinition { get; }
        public override bool IsGenericParameter { get; }
        public override TypeMember GenericTypeDefinition { get; }
        public override ImmutableList<TypeMember> GenericArguments { get; }
        public override ImmutableList<TypeMember> GenericParameters { get; }
        public override TypeMember UnderlyingType { get; }
        public override ImmutableList<AbstractMember> Members { get; }
        public override TypeGeneratorInfo Generator { get; }

        public override bool Equals(TypeMember other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            //TODO: compare by bindings
            //if (this.ClrBinding != null)
            //{
            //    return (this.ClrBinding == other.ClrBinding);
            //}

            return ReferenceEquals(this, other);
        }

        public override int GetHashCode()
        {
            //TODO: calculate from bindings
            //if (ClrBinding != null)
            //{
            //    return 127 ^ ClrBinding.GetHashCode();
            //}

            return 17 ^ base.GetHashCode();
        }

        public override TypeMember MakeGenericType(params TypeMember[] typeArguments)
        {
            //TODO: validate type arguments

            var mutator = new TypeMemberMutator(new TypeMemberMutatorBuilder
            {
                IsGenericType = true,
                GenericTypeDefinition = this,
                GenericArguments = typeArguments.ToList()
            });

            return new RealTypeMember(this, mutator);
        }

        public override void AcceptVisitor(MemberVisitor visitor)
        {
            base.AcceptVisitor(visitor);

            switch (this.TypeKind)
            {
                case TypeMemberKind.Class:
                    visitor.VisitClassType(this);
                    break;
                case TypeMemberKind.Struct:
                    visitor.VisitStructType(this);
                    break;
                case TypeMemberKind.Interface:
                    visitor.VisitInterfaceType(this);
                    break;
                case TypeMemberKind.Enum:
                    visitor.VisitEnumType(this);
                    break;
            }

            foreach (var member in this.Members)
            {
                member.AcceptVisitor(visitor);
            }
        }

        protected internal override bool IsProxy => false;
        protected internal override TypeMember RealType => this;
    }
}
