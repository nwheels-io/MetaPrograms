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
            this.IsGenericTypeDefinition = this.IsGenericTypeDefinition.Assign(builder.IsGenericTypeDefinition);
            this.GenericTypeDefinition = this.GenericTypeDefinition.Assign(builder.GenericTypeDefinition);
            this.GenericTypeArguments = this.GenericTypeArguments.Assign(builder.GenericTypeArguments?.ToImmutableList());
            this.GenericTypeParameters = this.GenericTypeParameters.Assign(builder.GenericTypeParameters?.ToImmutableList());
            this.UnderlyingType = this.UnderlyingType.Assign(builder.UnderlyingType);
            this.Members = this.Members.Assign(builder.Members?.ToImmutableList());
            this.Generator = this.Generator.Assign(builder.Generator);
        }

        public Mutator<string>? Name { get; set; }
        public Mutator<TypeMember>? DeclaringType { get; set; }
        public Mutator<MemberStatus>? Status { get; set; }
        public Mutator<MemberVisibility>? Visibility { get; set; }
        public Mutator<MemberModifier>? Modifier { get; set; }
        public Mutator<ImmutableList<AttributeDescription>>? Attributes { get; set; }
        public Mutator<string>? AssemblyName { get; set; }
        public Mutator<string>? Namespace { get; set; }
        public Mutator<TypeMember>? BaseType { get; set; }
        public Mutator<ImmutableHashSet<TypeMember>>? Interfaces { get; set; }
        public Mutator<TypeMemberKind>? TypeKind { get; set; }
        public Mutator<bool>? IsAbstract { get; set; }
        public Mutator<bool>? IsValueType { get; set; }
        public Mutator<bool>? IsCollection { get; set; }
        public Mutator<bool>? IsArray { get; set; }
        public Mutator<bool>? IsNullable { get; set; }
        public Mutator<bool>? IsAwaitable { get; set; }
        public Mutator<bool>? IsGenericType { get; set; }
        public Mutator<bool>? IsGenericTypeDefinition { get; set; }
        public Mutator<TypeMember>? GenericTypeDefinition { get; set; }
        public Mutator<ImmutableList<TypeMember>>? GenericTypeArguments { get; set; }
        public Mutator<ImmutableList<TypeMember>>? GenericTypeParameters { get; set; }
        public Mutator<TypeMember>? UnderlyingType { get; set; }
        public Mutator<ImmutableList<AbstractMember>>? Members { get; set; }
        public Mutator<TypeGeneratorInfo>? Generator { get; set; }
    }
}
