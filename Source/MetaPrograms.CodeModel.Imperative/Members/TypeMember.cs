using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace MetaPrograms.CodeModel.Imperative.Members
{
    public abstract class TypeMember : AbstractMember
    {
        protected TypeMember(TypeMemberBuilder builder, MemberRefState selfReference = null)
            : base(
                builder.Name, 
                builder.DeclaringType, 
                builder.Status, 
                builder.Visibility, 
                builder.Modifier, 
                builder.Attributes.ToImmutableList(),
                selfReference)
        {
        }

        protected TypeMember(TypeMember source, TypeMemberMutator mutator)
            : base(
                source,
                name: mutator.Name,
                declaringType: mutator.DeclaringType,
                status: mutator.Status,
                visibility: mutator.Visibility,
                modifier: mutator.Modifier,
                attributes: mutator.Attributes)
        {
        }

        public abstract string AssemblyName { get; }
        public abstract string Namespace { get; }
        public abstract MemberRef<TypeMember> BaseType { get; }
        public abstract ImmutableHashSet<MemberRef<TypeMember>> Interfaces { get; }
        public abstract TypeMemberKind TypeKind { get; }
        public abstract bool IsAbstract { get; }
        public abstract bool IsValueType { get; }
        public abstract bool IsCollection { get; }
        public abstract bool IsArray { get; }
        public abstract bool IsNullable { get; }
        public abstract bool IsAwaitable { get; }
        public abstract bool IsGenericType { get; }
        public abstract bool IsGenericDefinition { get; }
        public abstract bool IsGenericParameter { get; }
        public abstract MemberRef<TypeMember> GenericTypeDefinition { get; }
        public abstract ImmutableList<MemberRef<TypeMember>> GenericArguments { get; }
        public abstract ImmutableList<MemberRef<TypeMember>> GenericParameters { get; }
        public abstract MemberRef<TypeMember> UnderlyingType { get; }
        public abstract ImmutableList<MemberRef<AbstractMember>> Members { get; }
        public abstract TypeGeneratorInfo Generator { get; }

        public MemberRef<TypeMember> GetRef() => new MemberRef<TypeMember>(SelfReference);

        public abstract bool Equals(TypeMember other);

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
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return $"{this.TypeKind} {this.Name}";
        }

        public abstract TypeMember MakeGenericType(params TypeMember[] typeArguments);

        public TypeMemberBuilder CreateCompletionBuilder()
        {
            return new TypeMemberBuilder(SelfReference);
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
                    string.Join(commaSeparator, GenericParameters.Select(t => t.Get().Name)) +
                    closeBracket);
            }
            else
            {
                return (
                    this.Name +
                    openBracket + 
                    string.Join(commaSeparator, GenericArguments.Select(t => t.Get().MakeGenericName(openBracket, closeBracket, commaSeparator))) +
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

        public string FullName
        {
            get
            {
                if (DeclaringType.IsNotNull)
                {
                    return DeclaringType.Get().FullName + "." + Name;
                }

                if (!string.IsNullOrEmpty(Namespace))
                {
                    return Namespace + "." + Name;
                }

                return Name;
            }
        }

        public static readonly MemberRef<TypeMember> Void = 
            new RealTypeMember(
                new TypeMemberBuilder() {
                    Status = MemberStatus.Compiled,
                    TypeKind = TypeMemberKind.Void,
                    Name = "void"
                }
            ).GetRef();

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
