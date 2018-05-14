using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace MetaPrograms.CodeModel.Imperative.Members
{
    public class TypeMember : AbstractMember, IEquatable<TypeMember>
    {
        public TypeMember(TypeMemberBuilder builder)
            : base(
                builder.Name, 
                builder.DeclaringType, 
                builder.Status, 
                builder.Visibility, 
                builder.Modifier, 
                builder.Attributes.ToImmutableList())
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

        public TypeMember(TypeMember source, TypeMemberMutator mutator)
            : base(
                source, 
                name: mutator.Name, 
                declaringType: mutator.DeclaringType, 
                status: mutator.Status, 
                visibility: mutator.Visibility, 
                modifier: mutator.Modifier, 
                attributes: mutator.Attributes)
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

        public string AssemblyName { get; }
        public string Namespace { get; }
        public TypeMember BaseType { get; }
        public ImmutableHashSet<TypeMember> Interfaces { get; }
        public TypeMemberKind TypeKind { get; }
        public bool IsAbstract { get; }
        public bool IsValueType { get; }
        public bool IsCollection { get; }
        public bool IsArray { get; }
        public bool IsNullable { get; }
        public bool IsAwaitable { get; }
        public bool IsGenericType { get; }
        public bool IsGenericDefinition { get; }
        public bool IsGenericParameter { get; }
        public TypeMember GenericTypeDefinition { get; }
        public ImmutableList<TypeMember> GenericArguments { get; }
        public ImmutableList<TypeMember> GenericParameters { get; }
        public TypeMember UnderlyingType { get; }
        public ImmutableList<AbstractMember> Members { get; }
        public TypeGeneratorInfo Generator { get; }

        public bool Equals(TypeMember other)
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

        public override bool Equals(object obj)
        {
            if (obj is TypeMember other)
            {
                return this.Equals(other);
            }

            return base.Equals(obj);
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

        public override string ToString()
        {
            return $"{this.TypeKind} {this.Name}";
        }

        public TypeMember MakeGenericType(params TypeMember[] typeArguments)
        {
            //TODO: validate type arguments

            var mutator = new TypeMemberMutator(new TypeMemberMutatorBuilder {
                IsGenericType = true,
                GenericTypeDefinition = this,
                GenericArguments = typeArguments.ToList()
            });

            return new TypeMember(this, mutator);
        }

        public string MakeGenericName(string openBracket, string closeBracket, string commaSeparator)
        {
            if (!IsGenericType)
            {
                return this.Name;
            }

            if (IsGenericDefinition)
            {
                return (
                    this.Name + 
                    openBracket + 
                    string.Join(commaSeparator, GenericParameters.Select(t => t.Name)) +
                    closeBracket);
            }
            else
            {
                return (
                    this.Name +
                    openBracket + 
                    string.Join(commaSeparator, GenericArguments.Select(t => t.MakeGenericName(openBracket, closeBracket, commaSeparator))) +
                    closeBracket);
            }
        }

        public string MakeNameWithGenericArity(char aritySeparator)
        {
            if (IsGenericDefinition)
            {
                return (this.Name + aritySeparator + GenericParameters.Count);
            }
            else if (IsGenericType)
            {
                return (this.Name + aritySeparator + GenericArguments.Count);
            }
            else
            {
                return this.Name;
            }
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

        public string FullName
        {
            get
            {
                if (DeclaringType != null)
                {
                    return DeclaringType.FullName + "." + Name;
                }

                if (!string.IsNullOrEmpty(Namespace))
                {
                    return Namespace + "." + Name;
                }

                return Name;
            }
        }


        public static bool operator == (TypeMember member1, TypeMember member2)
        {
            if (!ReferenceEquals(member1, null))
            {
                return member1.Equals(member2);
            }
            else
            {
                return ReferenceEquals(member2, null);
            }
        }

        public static bool operator != (TypeMember member1, TypeMember member2)
        {
            return !(member1 == member2);
        }
    }
}
