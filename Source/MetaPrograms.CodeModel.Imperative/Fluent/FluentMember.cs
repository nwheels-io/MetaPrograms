using System;
using System.Collections.Immutable;
using MetaPrograms.CodeModel.Imperative.Members;
using static MetaPrograms.CodeModel.Imperative.CodeGeneratorContext;

// ReSharper disable InconsistentNaming

namespace MetaPrograms.CodeModel.Imperative.Fluent
{
    public class FluentMember
    {
        public FluentMember(bool? isAsync = null, bool? isReadOnly = null)
        {
            var traits = CodeGeneratorContext.GetContextOrThrow().PeekStateOrThrow<MemberTraitsContext>();

            if (isAsync.HasValue)
            {
                traits.IsAsync = isAsync.Value;
            }

            if (isReadOnly.HasValue)
            {
                traits.IsReadonly = isReadOnly.Value;
            }
        }

        protected FluentMember()
        {
        }

        public void FIELD(TypeMember type, string name, out FieldMember @ref, Action body = null)
        {
            var context = CodeGeneratorContext.GetContextOrThrow();
            var traits = context.PopStateOrThrow<MemberTraitsContext>();
            var declaringTypeRef = context.TryLookupState<MemberRef<TypeMember>>();
            var member = new FieldMember(
                name,
                declaringTypeRef,
                MemberStatus.Generator,
                traits.Visibility,
                traits.Modifier,
                ImmutableList<AttributeDescription>.Empty,
                type.GetRef(),
                traits.IsReadonly,
                initializer: null);

            context.GetCurrentTypeBuilder().Members.Add(member.GetAbstractRef());
            
            using (context.PushState(member.GetRef()))
            {
                body?.Invoke();
            }

            @ref = member;
        }

        public void FIELD<TType>(string name, out FieldMember @ref, Action body = null)
        {
            var type = CodeGeneratorContext.GetContextOrThrow().FindMemberOrThrow<TypeMember>(binding: typeof(TType));
            FIELD(type, name, out @ref, body);
        }

        public TypeMember CLASS(string name, Action body) 
            => FluentHelpers.BuildTypeMember(TypeMemberKind.Class, name, body);

        public TypeMember STRUCT(string name, Action body) 
            => FluentHelpers.BuildTypeMember(TypeMemberKind.Struct, name, body);

        public TypeMember INTERFACE(string name, Action body) 
            => FluentHelpers.BuildTypeMember(TypeMemberKind.Interface, name, body);

        public ConstructorMember CONSTRUCTOR(Action body)
            => new ConstructorGenerator(GetContextOrThrow(), body).GenerateMember();
        //
        // {
        //     var context = CodeGeneratorContext.GetContextOrThrow();
        //     var traits = GetContextOrThrow().PopStateOrThrow<MemberTraitsContext>();
        //     var declaringTypeBuilder = context.GetCurrentTypeBuilder();
        //     var declaringType = context.GetCurrentType(); //TODO: why declaringType is null?
        //     var member = new ConstructorMember(
        //         declaringTypeBuilder.GetTemporaryProxy().GetRef(),   
        //         MemberStatus.Generator, 
        //         traits.Visibility,
        //         traits.Modifier,
        //         ImmutableList<AttributeDescription>.Empty,
        //         new MethodSignature(
        //             isAsync: false, 
        //             returnValue: null, 
        //             parameters: ImmutableList<MethodParameter>.Empty),
        //         body: null,
        //         callThisConstructor: null,
        //         callBaseConstructor: null);
        //     
        //     declaringTypeBuilder.Members.Add(member.GetAbstractRef());
        //
        //     using (context.PushState(member.GetRef()))
        //     {
        //         body?.Invoke();
        //     }
        //
        //     return member;
        // }

        public MethodMember FUNCTION<TReturnType>(string name, Action body)
            => new MethodGenerator(GetContextOrThrow(), typeof(TReturnType), name, body).GenerateMember();
        
        // {
        //     var context = CodeGeneratorContext.GetContextOrThrow();
        //     var traits = GetContextOrThrow().PopStateOrThrow<MemberTraitsContext>();
        //     var declaringTypeBuilder = context.GetCurrentTypeBuilder();
        //     var returnType = context.FindType<TReturnType>();
        //     var declaringType = context.GetCurrentType(); //TODO: why declaringType is null?
        //     var member = new MethodMember(
        //         name,
        //         declaringTypeBuilder.GetTemporaryProxy().GetRef(),   
        //         MemberStatus.Generator, 
        //         traits.Visibility,
        //         traits.Modifier,
        //         ImmutableList<AttributeDescription>.Empty,
        //         new MethodSignature(
        //             isAsync: traits.IsAsync, 
        //             returnValue: new MethodParameter(null, 0, returnType, MethodParameterModifier.None, ImmutableList<AttributeDescription>.Empty), 
        //             parameters: ImmutableList<MethodParameter>.Empty),
        //         body: null);
        //     
        //     declaringTypeBuilder.Members.Add(member.GetAbstractRef());
        //
        //     using (context.PushState(member.GetRef()))
        //     {
        //         body?.Invoke();
        //     }
        //
        //     return member;
        // }

        public MethodMember FUNCTION(MemberRef<TypeMember> returnType, string name, Action body)
            => new MethodGenerator(GetContextOrThrow(), returnType, name, body).GenerateMember();

        // {
        //     GetContextOrThrow().PopStateOrThrow<MemberTraitsContext>();
        //     return null;
        // }

        public MethodMember VOID(string name, Action body)
            => new MethodGenerator(GetContextOrThrow(), name, body).GenerateMember();

        // {
        //     GetContextOrThrow().PopStateOrThrow<MemberTraitsContext>();
        //     return null;
        // }

        public MethodMember VOID(MemberRef<MethodMember> ancestorMethod, Action body)
            => new MethodGenerator(GetContextOrThrow(), ancestorMethod, body).GenerateMember();

        // {
        //     GetContextOrThrow().PopStateOrThrow<MemberTraitsContext>();
        //     return null;
        // }

        public void PROPERTY<T>(string name, Action body = null)
        {
            GetContextOrThrow().PopStateOrThrow<MemberTraitsContext>();
        }

        public void PROPERTY(TypeMember type, string name, Action body = null)
        {
            GetContextOrThrow().PopStateOrThrow<MemberTraitsContext>();
        }
    }
}