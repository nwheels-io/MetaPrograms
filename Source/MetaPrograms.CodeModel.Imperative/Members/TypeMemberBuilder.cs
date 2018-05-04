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
            this.GenericTypeArguments = new List<TypeMember>();
            this.GenericTypeParameters = new List<TypeMember>();
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
        public bool IsGenericTypeDefinition { get; set; }
        public TypeMember GenericTypeDefinition { get; set; }
        public List<TypeMember> GenericTypeArguments { get; }
        public List<TypeMember> GenericTypeParameters { get; }
        public TypeMember UnderlyingType { get; set; }
        public List<AbstractMember> Members { get; }
        public TypeGeneratorInfo Generator { get; set; }
    }
}
