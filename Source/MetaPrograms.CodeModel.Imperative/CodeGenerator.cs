#if false

using System;
using System.Collections.Immutable;
using System.Reflection;
using MetaPrograms.CodeModel.Imperative.Expressions;
using MetaPrograms.CodeModel.Imperative.Members;
using static MetaPrograms.CodeModel.Imperative.CodeGeneratorContext;

// ReSharper disable InconsistentNaming

namespace MetaPrograms.CodeModel.Imperative
{
    public static class CodeGenerator
    {
        public static void NAMESPACE(string name, Action body)
        {
            using (GetContextOrThrow().PushState(new NamespaceState(name)))
            {
                body();
            }
        }

        public static FluentModifier1 PUBLIC => new FluentModifier1(MemberVisibility.Public);
        public static FluentModifier1 PRIVATE => new FluentModifier1(MemberVisibility.Private);
        public static FluentModifier1 PROTECTED => new FluentModifier1(MemberVisibility.Protected);
        public static FluentModifier1 INTERNAL => new FluentModifier1(MemberVisibility.Internal);

        public class FluentModifier1 : FluentModifier2
        {
            public FluentModifier1(MemberVisibility visibility)
            {
                GetContextOrThrow().PushState(new MemberModifierState(visibility));
            }

            public FluentModifier2 STATIC => new FluentModifier2(MemberModifier.Static);
            public FluentModifier2 ABSTRACT => new FluentModifier2(MemberModifier.Abstract);
            public FluentModifier2 VIRTUAL => new FluentModifier2(MemberModifier.Virtual);
            public FluentModifier2 OVERRIDE => new FluentModifier2(MemberModifier.Override);
        }

        public class FluentModifier2 : FluentMember
        {
            public FluentModifier2(MemberModifier modifier)
            {
                GetContextOrThrow().PeekStateOrThrow<MemberModifierState>().Modifier = modifier;
            }

            protected FluentModifier2()
            {
            }

            public FluentMember ASYNC => new FluentMember(isAsync: true);
            public FluentMember READONLY => new FluentMember(isReadOnly: true);
        }

        public class FluentMember
        {
            public FluentMember(bool? isAsync = null, bool? isReadOnly = null)
            {
                var state = GetContextOrThrow().PeekStateOrThrow<MemberModifierState>();

                if (isAsync.HasValue)
                {
                    state.IsAsync = isAsync.Value;
                }

                if (isReadOnly.HasValue)
                {
                    state.IsReadonly = isReadOnly.Value;
                }
            }

            protected FluentMember()
            {
            }

            public FieldMember FIELD(TypeMember type, string name, Action body = null)
            {
                var context = GetContextOrThrow();
                var modifiers = context.PopStateOrThrow<MemberModifierState>();
                var declaringTypeRef = context.TryLookupState<MemberRef<TypeMember>>();
                var member = new FieldMember(
                    name,
                    declaringTypeRef,
                    MemberStatus.Generator,
                    modifiers.Visibility,
                    modifiers.Modifier,
                    ImmutableList<AttributeDescription>.Empty,
                    type.GetRef(),
                    modifiers.IsReadonly,
                    initializer: null);

                using (context.PushState(member.GetRef()))
                {
                    body?.Invoke();
                }

                return member;
            }

            public FieldMember FIELD<TType>(string name, Action body = null)
            {
                var type = GetContextOrThrow().FindMemberOrThrow<TypeMember>(binding: typeof(TType));
                return FIELD(type, name, body);
            }

            public TypeMember CLASS(string name, Action body) => BuildType(TypeMemberKind.Class, name, body);

            public TypeMember STRUCT(string name, Action body) => BuildType(TypeMemberKind.Struct, name, body);

            public TypeMember INTERFACE(string name, Action body) => BuildType(TypeMemberKind.Interface, name, body);

            public void CONSTRUCTOR(Action body) { }

            public void FUNCTION<TReturnType>(string name, Action body) { }
            public void FUNCTION(TypeMember returnType, string name) { }

            public void VOID(string name, Action body) { }
            public void VOID(MethodMember ancestorMethod, Action body) { }

            public FluentProperty PROPERTY<T>(string name, Action body = null) => null;
            public FluentProperty PROPERTY(TypeMember type, string name, Action body = null) => null;

            public static TypeMember BuildType(TypeMemberKind typeKind, string name, Action body)
            {
                var context = GetContextOrThrow();
                var modifiers = context.PopStateOrThrow<MemberModifierState>();
                var namespaceState = context.TryLookupState<NamespaceState>();
                var containingTypeRef = context.TryLookupState<MemberRef<TypeMember>>();

                var builder = new TypeMemberBuilder();
                builder.Namespace = namespaceState?.Name;
                builder.Name = name;
                builder.TypeKind = typeKind;
                builder.DeclaringType = containingTypeRef;

                using (context.PushState(builder.UnderlyingType))
                {
                    body?.Invoke();
                }

                var finalMember = new RealTypeMember(builder);
                builder.GetMemberSelfReference().Reassign(finalMember);
                return finalMember;
            }
        }


        public static void ATTRIBUTE<T>(params object[] constructorArgumentsAndBody) { }
        public static void ATTRIBUTE(TypeMember type, params object[] constructorArgumentsAndBody) { }

        public static void EXTENDS<T>() {  }
        public static void EXTENDS(TypeMember type) { }

        public static void PARAMETER<T>(string name, out MethodParameter @ref, Action body = null)
        {
            @ref = null;
        }

        public static void PARAMETER(TypeMember type, string name, out MethodParameter @ref, Action body = null)
        {
            @ref = null;
        }

        public static void LOCAL(TypeMember type, string name, out LocalVariable @ref)
        {
            @ref = null;
        }

        public static void LOCAL<T>(string name, out LocalVariable @ref)
        {
            @ref = null;
        }

        public static void AUTOMATIC() { }
        public static void AUTOMATIC(FieldMember field) { }
        
        public static void ARGUMENT(AbstractExpression value) { }

        public static AbstractExpression AWAIT(AbstractExpression promiseExpression) => null;
        public static FluentStatement DO => null;
        public static ThisExpression THIS => null;
        
        public static AbstractExpression DOT(this AbstractExpression target, AbstractMember member) => null;
        public static AbstractExpression DOT(this AbstractExpression target, string memberName) => null;
        public static AbstractExpression DOT(this LocalVariable target, AbstractMember member) => null;
        public static AbstractExpression DOT(this LocalVariable target, string memberName) => null;
        public static AbstractExpression DOT(this MethodParameter target, AbstractMember member) => null;
        public static AbstractExpression DOT(this MethodParameter target, string memberName) => null;

        public static AbstractExpression NOT(AbstractExpression value) => null;
        public static AbstractExpression NEW<T>(params object[] constructorArguments) => null;
        public static AbstractExpression NEW(TypeMember type, params object[] constructorArguments) => null;
        
        public static AbstractExpression ASSIGN(this AbstractExpression target, AbstractExpression value) => null;
        public static AbstractExpression ASSIGN(this AbstractMember target, AbstractExpression value) => null;
        public static AbstractExpression ASSIGN(this IAssignable target, AbstractExpression value) => null;

        public static AbstractExpression INVOKE(this AbstractExpression target, params AbstractExpression[] arguments) => null;
        public static AbstractExpression INVOKE(this AbstractExpression target, Action body) => null;


        public class FluentStatement
        {
            public void RETURN(AbstractExpression value) { }
            public FluentIf IF(AbstractExpression condition) => null;
        }

        public class FluentProperty
        {
            
        }

        public class FluentIf
        {
            public FluentElse THEN(Action body) => null;
        }

        public class FluentElse
        {
            public FluentElse ELSEIF(AbstractExpression condition) => null;
            public void ELSE(AbstractExpression condition) { }
        }

        public class NamespaceState
        {
            public NamespaceState(string name)
            {
                Name = name;
            }

            public string Name { get; }
        }

        public class MemberModifierState
        {
            public MemberModifierState(MemberVisibility visibility)
            {
                Visibility = visibility;
                Modifier = MemberModifier.None;
            }

            public MemberVisibility Visibility { get; }
            public MemberModifier Modifier { get; set; }
            public bool IsAsync { get; set; }
            public bool IsReadonly { get; set; }
        }
    }
}


#endif
