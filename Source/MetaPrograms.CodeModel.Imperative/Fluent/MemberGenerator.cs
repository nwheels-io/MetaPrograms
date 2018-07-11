using System;
using System.Collections.Immutable;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Fluent
{
    public abstract class MemberGenerator<TMember>
        where TMember : AbstractMember, new()
    {
        public CodeGeneratorContext Context { get; }
        public MemberTraitsContext Traits { get; }
        public ModuleMember DeclaringModule { get; }
        public TypeMember DeclaringType { get; }
        public string Name { get; }
        public Action Body { get; }

        protected MemberGenerator(CodeGeneratorContext context, string name, Action body)
        {
            this.Context = context;
            this.Name = name;
            this.Body = body;
            this.Traits = context.PopStateOrThrow<MemberTraitsContext>();;
            this.DeclaringModule = context.TryGetCurrentModule();
            this.DeclaringType = context.TryGetCurrentType();
        }

        public TMember GenerateMember()
        {
            var member = new TMember();
            InitializeMember(member);

            if (DeclaringType != null)
            {
                this.DeclaringType.Members.Add(member);
            }
            else if (DeclaringModule != null)
            {
                this.DeclaringModule.Members.Add(member);
            }

            using (Context.PushState(member))
            {
                using (Context.PushState(CreateStateAroundBody(member)))
                {
                    Body?.Invoke();
                }
            }

            return member;
        }

        protected virtual void InitializeMember(TMember member)
        {
            member.DeclaringType = this.DeclaringType;
            member.Name = this.Name;
            member.Status = MemberStatus.Generator;
            member.Visibility = Traits.Visibility;
            member.Modifier = Traits.Modifier;
        }

        protected virtual IDisposable CreateStateAroundBody(TMember member) => null;
    }

    public class FieldGenerator : MemberGenerator<FieldMember>
    {
        public TypeMember FieldType { get; }

        public FieldGenerator(CodeGeneratorContext context, Type fieldType, string name, Action body)
            : this(context, context.FindType(fieldType), name, body)
        {
        }

        public FieldGenerator(CodeGeneratorContext context, TypeMember fieldType, string name, Action body)
            : base(context, name, body)
        {
            this.FieldType = fieldType;
        }

        protected override void InitializeMember(FieldMember member)
        {
            base.InitializeMember(member);
            member.Type = FieldType;
            member.IsReadOnly = Traits.IsReadOnly;
        }
    }

    public class PropertyGenerator : MemberGenerator<PropertyMember>
    {
        public TypeMember PropertyType { get; }

        public PropertyGenerator(CodeGeneratorContext context, TypeMember propertyType, string name, Action body)
            : base(context, name, body)
        {
            this.PropertyType = propertyType;
        }

        public PropertyGenerator(CodeGeneratorContext context, Type propertyType, string name, Action body)
            : this(context, context.FindType(propertyType), name, body)
        {
        }

        protected override void InitializeMember(PropertyMember member)
        {
            base.InitializeMember(member);
            member.PropertyType = this.PropertyType;
        }
    }
    
    public class ConstructorGenerator : MemberGenerator<ConstructorMember>
    {
        public ConstructorGenerator(CodeGeneratorContext context, Action body)
            : base(context, name: "constructor", body)
        {
        }

        protected override void InitializeMember(ConstructorMember member)
        {
            base.InitializeMember(member);
            member.Signature = new MethodSignature();
            member.Body = new Statements.BlockStatement();
        }

        protected override IDisposable CreateStateAroundBody(ConstructorMember member)
        {
            return new BlockContext(member.Body);
        }
    }
    
    public class MethodGenerator : MemberGenerator<MethodMember>
    {
        public TypeMember ReturnType { get; }
        public MethodMember AncestorMethod { get; }
        
        public MethodGenerator(CodeGeneratorContext context, string name, Action body)
            : this(context, returnType: (Type)null, name, body)
        {
        }

        public MethodGenerator(CodeGeneratorContext context, Type returnType, string name, Action body)
            : base(context, name, body)
        {
            this.ReturnType = Context.FindType(returnType);
        }

        public MethodGenerator(CodeGeneratorContext context, TypeMember returnType, string name, Action body)
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

        protected override void InitializeMember(MethodMember member)
        {
            base.InitializeMember(member);
            member.Signature = CreateSignature();
            member.Body = new Statements.BlockStatement();
        }

        protected override IDisposable CreateStateAroundBody(MethodMember member)
        {
            return new BlockContext(member.Body);
        }

        private MethodSignature CreateSignature() => (
            AncestorMethod != null
                ? AncestorMethod.Signature
                : new MethodSignature { 
                    IsAsync = Traits.IsAsync,
                    ReturnValue = ReturnValueParameter
                }
        );
 
        private MethodParameter ReturnValueParameter => (
            ReturnType != null
            ? new MethodParameter { Type = ReturnType }
            : null
        );
    }
}
