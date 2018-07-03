using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace MetaPrograms.CodeModel.Imperative.Members
{
    public class TypeMemberMutator
    {
        public TypeMemberMutator(TypeMemberMutatorBuilder builder)
        {
            this.Name = this.Name.Assign(builder.Name);
            this.DeclaringType = this.DeclaringType.Assign(builder.DeclaringType);
            this.Status = this.Status.Assign(builder.Status);
            this.Visibility = this.Visibility.Assign(builder.Visibility);
            this.Modifier = this.Modifier.Assign(builder.Modifier);
            this.Attributes = this.Attributes.Assign(builder.Attributes?.ToImmutableList());
            this.AssemblyName = this.AssemblyName.Assign(builder.AssemblyName);
            this.ModuleName = this.ModuleName.Assign(builder.ModuleName);
            this.Namespace = this.Namespace.Assign(builder.Namespace);
            this.BaseType = this.BaseType.Assign(builder.BaseType);
            this.Interfaces = this.Interfaces.Assign(builder.Interfaces?.ToImmutableHashSet());
            this.TypeKind = this.TypeKind.Assign(builder.TypeKind);
            this.IsAbstract = this.IsAbstract.Assign(builder.IsAbstract);
            this.IsValueType = this.IsValueType.Assign(builder.IsValueType);
            this.IsCollection = this.IsCollection.Assign(builder.IsCollection);
            this.IsArray = this.IsArray.Assign(builder.IsArray);
            this.IsNullable = this.IsNullable.Assign(builder.IsNullable);
            this.IsAwaitable = this.IsAwaitable.Assign(builder.IsAwaitable);
            this.IsGenericType = this.IsGenericType.Assign(builder.IsGenericType);
            this.IsGenericDefinition = this.IsGenericDefinition.Assign(builder.IsGenericDefinition);
            this.IsGenericParameter = this.IsGenericParameter.Assign(builder.IsGenericParameter);
            this.GenericTypeDefinition = this.GenericTypeDefinition.Assign(builder.GenericTypeDefinition);
            this.GenericArguments = this.GenericArguments.Assign(builder.GenericArguments?.ToImmutableList());
            this.GenericParameters = this.GenericParameters.Assign(builder.GenericParameters?.ToImmutableList());
            this.UnderlyingType = this.UnderlyingType.Assign(builder.UnderlyingType);
            this.Imports = this.Imports.Assign(builder.Imports?.ToImmutableList());
            this.Members = this.Members.Assign(builder.Members?.ToImmutableList());
            this.Generator = this.Generator.Assign(builder.Generator);
        }

        public Mutator<string>? Name { get; set; }
        public Mutator<MemberRef<TypeMember>>? DeclaringType { get; set; }
        public Mutator<MemberStatus>? Status { get; set; }
        public Mutator<MemberVisibility>? Visibility { get; set; }
        public Mutator<MemberModifier>? Modifier { get; set; }
        public Mutator<ImmutableList<AttributeDescription>>? Attributes { get; set; }
        public Mutator<string>? AssemblyName { get; set; }
        public Mutator<string>? ModuleName { get; set; }
        public Mutator<string>? Namespace { get; set; }
        public Mutator<MemberRef<TypeMember>>? BaseType { get; set; }
        public Mutator<ImmutableHashSet<MemberRef<TypeMember>>>? Interfaces { get; set; }
        public Mutator<TypeMemberKind>? TypeKind { get; set; }
        public Mutator<bool>? IsAbstract { get; set; }
        public Mutator<bool>? IsValueType { get; set; }
        public Mutator<bool>? IsCollection { get; set; }
        public Mutator<bool>? IsArray { get; set; }
        public Mutator<bool>? IsNullable { get; set; }
        public Mutator<bool>? IsAwaitable { get; set; }
        public Mutator<bool>? IsGenericType { get; set; }
        public Mutator<bool>? IsGenericDefinition { get; set; }
        public Mutator<bool>? IsGenericParameter { get; set; }
        public Mutator<MemberRef<TypeMember>>? GenericTypeDefinition { get; set; }
        public Mutator<ImmutableList<MemberRef<TypeMember>>>? GenericArguments { get; set; }
        public Mutator<ImmutableList<MemberRef<TypeMember>>>? GenericParameters { get; set; }
        public Mutator<MemberRef<TypeMember>>? UnderlyingType { get; set; }
        public Mutator<ImmutableList<ImportDirective>>? Imports { get; set; }
        public Mutator<ImmutableList<MemberRef<AbstractMember>>>? Members { get; set; }
        public Mutator<TypeGeneratorInfo>? Generator { get; set; }

        public static implicit operator TypeMemberMutator(TypeMemberMutatorBuilder builder)
            => new TypeMemberMutator(builder);
    }
}
