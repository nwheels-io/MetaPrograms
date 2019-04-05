using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace MetaPrograms.Members
{
    public class TypeMember : AbstractMember
    {
        public string AssemblyName { get; set; }
        public string ModuleName { get; set; }
        public string Namespace { get; set; }
        public TypeMember BaseType { get; set; }
        public HashSet<TypeMember> Interfaces { get; } = new HashSet<TypeMember>();
        public TypeMemberKind TypeKind { get; set; }
        public bool IsValueType { get; set; }
        public bool IsCollection { get; set; }
        public bool IsArray { get; set; }
        public bool IsNullable { get; set; }
        public bool IsAwaitable { get; set; }
        public bool IsGenericType { get; set; }
        public bool IsGenericDefinition { get; set; }
        public bool IsGenericParameter { get; set; }
        public TypeMember GenericTypeDefinition { get; set; }
        public List<TypeMember> GenericArguments { get; set; } = new List<TypeMember>();
        public List<TypeMember> GenericParameters { get; } = new List<TypeMember>();
        public TypeMember UnderlyingType { get; set; }
        public List<AbstractMember> Members { get; } = new List<AbstractMember>();
        public TypeGeneratorInfo Generator { get; set; }

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

        public override string ToString()
        {
            return $"{this.TypeKind} {this.Name}";
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

        public bool IsAssignableFrom(TypeMember other)
        {
            while (!ReferenceEquals(other, null))
            {
                if (ReferenceEquals(other, this))
                {
                    return true;
                }

                other = other.BaseType;
            }

            return false;
        }
        
        public TypeMember MakeGenericType(params TypeMember[] typeArguments)
        {
            //TODO: validate type arguments

            return new TypeMember {
                IsGenericType = true,
                GenericTypeDefinition = this,
                GenericArguments = typeArguments.ToList()
            };
        }

        public string MakeGenericName(string openBracket, string closeBracket, string commaSeparator)
        {
            if (!IsGenericType)
            {
                return this.Name.ToString();
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
                return (this.Name.ToString() + aritySeparator + GenericParameters.Count);
            }
            else if (IsGenericType)
            {
                return (this.Name.ToString() + aritySeparator + GenericArguments.Count);
            }
            else
            {
                return this.Name.ToString();
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

                return Name.ToString();
            }
        }

        public static readonly TypeMember Void = new TypeMember {
            Status = MemberStatus.Compiled,
            TypeKind = TypeMemberKind.Void,
            Name = "void"
        };

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
