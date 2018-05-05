﻿using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using MetaPrograms.CodeModel.Imperative.Statements;

namespace MetaPrograms.CodeModel.Imperative.Members
{
    public abstract class MethodMemberBase : AbstractMember
    {
        protected MethodMemberBase(
            string name, 
            TypeMember declaringType, 
            MemberStatus status, 
            MemberVisibility visibility, 
            MemberModifier modifier, 
            ImmutableList<AttributeDescription> attributes, 
            MethodSignature signature, 
            BlockStatement body) 
            : base(name, declaringType, status, visibility, modifier, attributes)
        {
            Signature = signature;
            Body = body;
        }

        protected MethodMemberBase(
            MethodMemberBase source, 
            Mutator<string>? name = null, 
            Mutator<TypeMember>? declaringType = null, 
            Mutator<MemberStatus>? status = null, 
            Mutator<MemberVisibility>? visibility = null, 
            Mutator<MemberModifier>? modifier = null, 
            Mutator<ImmutableList<AttributeDescription>>? attributes = null,
            Mutator<MethodSignature>? signature = null,
            Mutator<BlockStatement>? body = null) 
            : base(source, name, declaringType, status, visibility, modifier, attributes)
        {
            Signature = signature.MutatedOrOriginal(source.Signature);
            Body = body.MutatedOrOriginal(source.Body);
        }

        public override void AcceptVisitor(MemberVisitor visitor)
        {
            base.AcceptVisitor(visitor);

            if (this.Signature != null)
            {
                if (this.Signature.Parameters != null)
                {
                    foreach (var parameter in this.Signature.Parameters.Where(p => p.Attributes != null))
                    {
                        foreach (var attribute in parameter.Attributes)
                        {
                            visitor.VisitAttribute(attribute);
                        }
                    }
                }

                if (this.Signature.ReturnValue != null && this.Signature.ReturnValue.Attributes != null)
                {
                    foreach (var attribute in this.Signature.ReturnValue.Attributes)
                    {
                        visitor.VisitAttribute(attribute);
                    }
                }
            }
        }

        public MethodSignature Signature { get; }
        public BlockStatement Body { get; }

        protected static MemberModifier GetMemberModifier(MethodBase binding)
        {
            var modifiers = MemberModifier.None;

            if (binding.IsStatic)
            {
                modifiers |= MemberModifier.Static;
            }

            if (binding.IsAbstract)
            {
                modifiers |= MemberModifier.Abstract;
            }

            if (binding.IsVirtual)
            {
                modifiers |= MemberModifier.Virtual;
            }

            return modifiers;
        }

        protected static MemberVisibility GetMemberVisibility(MethodBase binding)
        {
            if (binding.IsPrivate)
            {
                return MemberVisibility.Private;
            }

            if (binding.IsPublic)
            {
                return MemberVisibility.Public;
            }

            if (binding.IsFamilyAndAssembly)
            {
                return MemberVisibility.InternalProtected;
            }

            if (binding.IsFamily)
            {
                return MemberVisibility.Protected;
            }

            if (binding.IsAssembly)
            {
                return MemberVisibility.Internal;
            }

            throw new ArgumentException($"Visibility of member '{binding.Name}' cannot be determined.", nameof(binding));
        }
    }
}