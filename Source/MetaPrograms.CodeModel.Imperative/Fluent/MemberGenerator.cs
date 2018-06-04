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
                using (Context.PushState(CreateStateAroundBody()))
                {
                    Body?.Invoke();
                }
            }

            return member;
        }
        
        protected abstract TMember CreateMember();
        protected virtual IDisposable CreateStateAroundBody() => null;
        
        protected MemberRef<TypeMember> TypeBuilderRef => TypeBuilder.GetTemporaryProxy().GetRef();
        protected ImmutableList<AttributeDescription> EmptyAttributes => ImmutableList<AttributeDescription>.Empty;
        protected ImmutableList<MethodParameter> EmptyParameters => ImmutableList<MethodParameter>.Empty;
    }

    public class FieldGenerator : MemberGenerator<FieldMember>
    {
        public MemberRef<TypeMember> FieldType { get; }

        public FieldGenerator(CodeGeneratorContext context, Type fieldType, string name, Action body)
            : this(context, context.FindType(fieldType), name, body)
        {
        }

        public FieldGenerator(CodeGeneratorContext context, MemberRef<TypeMember> fieldType, string name, Action body)
            : base(context, name, body)
        {
            this.FieldType = fieldType;
        }

        protected override FieldMember CreateMember()
        {
            return new FieldMember(
                Name,
                TypeBuilderRef,
                MemberStatus.Generator,
                Traits.Visibility,
                Traits.Modifier,
                EmptyAttributes,
                FieldType,
                Traits.IsReadonly,
                initializer: null);
        }
    }

    public class PropertyGenerator : MemberGenerator<PropertyMember>
    {
        public MemberRef<TypeMember> PropertyType { get; }

        public PropertyGenerator(CodeGeneratorContext context, MemberRef<TypeMember> propertyType, string name, Action body)
            : base(context, name, body)
        {
            this.PropertyType = propertyType;
        }

        public PropertyGenerator(CodeGeneratorContext context, Type propertyType, string name, Action body)
            : this(context, context.FindType(propertyType), name, body)
        {
        }

        protected override PropertyMember CreateMember()
        {
            return new PropertyMember(
                Name,
                TypeBuilderRef,
                MemberStatus.Generator,
                Traits.Visibility,
                Traits.Modifier,
                EmptyAttributes,
                PropertyType,
                getter: MemberRef<MethodMember>.Null,
                setter: MemberRef<MethodMember>.Null);
        }
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

        protected override IDisposable CreateStateAroundBody()
        {
            return new MethodBodyContext(this.Context);
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

        protected override IDisposable CreateStateAroundBody()
        {
            return new MethodBodyContext(this.Context);
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
