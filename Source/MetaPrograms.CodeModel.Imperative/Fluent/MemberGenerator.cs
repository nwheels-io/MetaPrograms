using System;
using System.Collections.Immutable;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Fluent
{
    public abstract class MemberGenerator<TMember>
        where TMember : AbstractMember
    {
        public CodeGeneratorContext Context { get; }
        public MemberTraitsContext Traits { get; }
        public TypeMemberBuilder TypeBuilder { get; }
        public string Name { get; }
        public Action Body { get; }

        protected MemberGenerator(CodeGeneratorContext context, string name, Action body)
        {
            this.Context = context;
            this.Name = name;
            this.Body = body;
            this.Traits = context.PopStateOrThrow<MemberTraitsContext>();;
            this.TypeBuilder = context.GetCurrentTypeBuilder();
        }

        public TMember GenerateMember()
        {
            var member = CreateMember();
            
            TypeBuilder.Members.Add(member.GetAbstractRef());

            using (Context.PushState(member.GetAbstractRef()))
            {
                Body?.Invoke();
            }

            return member;
        }
        
        protected abstract TMember CreateMember();
        
        protected MemberRef<TypeMember> TypeBuilderRef => TypeBuilder.GetTemporaryProxy().GetRef();
        protected ImmutableList<AttributeDescription> EmptyAttributes => ImmutableList<AttributeDescription>.Empty;
        protected ImmutableList<MethodParameter> EmptyParameters => ImmutableList<MethodParameter>.Empty;
    }

    public class ConstructorGenerator : MemberGenerator<ConstructorMember>
    {
        public ConstructorGenerator(CodeGeneratorContext context, Action body)
            : base(context, name: "constructor", body)
        {
        }

        protected override ConstructorMember CreateMember()
        {
            return new ConstructorMember(
                TypeBuilderRef,   
                MemberStatus.Generator, 
                Traits.Visibility,
                Traits.Modifier,
                EmptyAttributes,
                new MethodSignature(
                    isAsync: Traits.IsAsync, 
                    returnValue: null, 
                    parameters: EmptyParameters),
                body: null,
                callThisConstructor: null,
                callBaseConstructor: null);
        }
    }
    
    public class MethodGenerator : MemberGenerator<MethodMember>
    {
        public MemberRef<TypeMember> ReturnType { get; }
        public MethodMember AncestorMethod { get; }
        
        public MethodGenerator(CodeGeneratorContext context, string name, Action body)
            : this(context, MemberRef<TypeMember>.Null, name, body)
        {
        }

        public MethodGenerator(CodeGeneratorContext context, Type returnType, string name, Action body)
            : base(context, name, body)
        {
            this.ReturnType = Context.FindType(returnType);
        }

        public MethodGenerator(CodeGeneratorContext context, MemberRef<TypeMember> returnType, string name, Action body)
            : base(context, name, body)
        {
            this.ReturnType = returnType;
        }

        public MethodGenerator(CodeGeneratorContext context, MethodMember ancestorMethod, Action body)
            : base(context, ancestorMethod.Name, body)
        {
            this.AncestorMethod = ancestorMethod;
            this.ReturnType = ancestorMethod.ReturnType;
        }

        protected override MethodMember CreateMember()
        {
            return new MethodMember(
                Name,
                TypeBuilderRef,   
                MemberStatus.Generator, 
                Traits.Visibility,
                Traits.Modifier,
                EmptyAttributes,
                CreateSignature(),
                body: null);
        }

        private MethodSignature CreateSignature() => (
            AncestorMethod != null
                ? AncestorMethod.Signature
                : new MethodSignature(
                    isAsync: Traits.IsAsync,
                    returnValue: ReturnValueParameter,
                    parameters: EmptyParameters)
        );
 
        private MethodParameter ReturnValueParameter => (
            ReturnType.IsNotNull
            ? new MethodParameter(null, 0, ReturnType, MethodParameterModifier.None, EmptyAttributes)
            : null);
    }
}