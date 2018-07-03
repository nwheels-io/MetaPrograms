using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace MetaPrograms.CodeModel.Imperative.Members
{
    public class TypeMemberMutatorBuilder
    {
        public string Name { get; set; }
        public MemberRef<TypeMember>? DeclaringType { get; set; }
        public MemberStatus? Status { get; set; }
        public MemberVisibility? Visibility { get; set; }
        public MemberModifier? Modifier { get; set; }
        public List<AttributeDescription> Attributes { get; set; }
        public string AssemblyName { get; set; }
        public string ModuleName { get; set; }
        public string Namespace { get; set; }
        public MemberRef<TypeMember>? BaseType { get; set; }
        public HashSet<MemberRef<TypeMember>> Interfaces { get; set; }
        public TypeMemberKind? TypeKind { get; set; }
        public bool? IsAbstract { get; set; }
        public bool? IsValueType { get; set; }
        public bool? IsCollection { get; set; }
        public bool? IsArray { get; set; }
        public bool? IsNullable { get; set; }
        public bool? IsAwaitable { get; set; }
        public bool? IsGenericType { get; set; }
        public bool? IsGenericDefinition { get; set; }
        public bool? IsGenericParameter { get; set; }
        public MemberRef<TypeMember>? GenericTypeDefinition { get; set; }
        public List<MemberRef<TypeMember>> GenericArguments { get; set; }
        public List<MemberRef<TypeMember>> GenericParameters { get; set; }
        public MemberRef<TypeMember>? UnderlyingType { get; set; }
        public List<ImportDirective> Imports { get; set; }
        public List<MemberRef<AbstractMember>> Members { get; set; }
        public TypeGeneratorInfo? Generator { get; set; }
    }
}
