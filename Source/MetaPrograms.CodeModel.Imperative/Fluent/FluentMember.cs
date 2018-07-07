using System;
using System.Collections.Immutable;
using MetaPrograms.CodeModel.Imperative.Members;
using static MetaPrograms.CodeModel.Imperative.CodeGeneratorContext;

// ReSharper disable InconsistentNaming

namespace MetaPrograms.CodeModel.Imperative.Fluent
{
    public class FluentMember
    {
        public FluentMember(bool? isAsync = null, bool? isReadOnly = null, bool? isDefaultExport = null)
        {
            var traits = CodeGeneratorContext.GetContextOrThrow().PeekStateOrThrow<MemberTraitsContext>();

            if (isAsync.HasValue)
            {
                traits.IsAsync = isAsync.Value;
            }

            if (isReadOnly.HasValue)
            {
                traits.IsReadOnly = isReadOnly.Value;
            }

            if (isDefaultExport.HasValue)
            {
                traits.IsDefaultExport = isDefaultExport.Value;
            }
        }

        protected FluentMember()
        {
        }

        public void FIELD(TypeMember type, string name, out FieldMember @ref, Action body = null)
            => @ref = new FieldGenerator(GetContextOrThrow(), type, name, body).GenerateMember();

        public void FIELD(Type type, string name, out FieldMember @ref, Action body = null)
            => @ref = new FieldGenerator(GetContextOrThrow(), type, name, body).GenerateMember();

        public void FIELD(string name, out FieldMember @ref, Action body = null)
            => @ref = new FieldGenerator(GetContextOrThrow(), fieldType: (TypeMember)null, name, body).GenerateMember();


        public void FIELD<TType>(string name, out FieldMember @ref, Action body = null)
            => @ref = new FieldGenerator(GetContextOrThrow(), typeof(TType), name, body).GenerateMember();

        public TypeMember CLASS(string name, Action body) 
            => FluentHelpers.BuildTypeMember(TypeMemberKind.Class, name, body);

        public TypeMember STRUCT(string name, Action body) 
            => FluentHelpers.BuildTypeMember(TypeMemberKind.Struct, name, body);

        public TypeMember INTERFACE(string name, Action body) 
            => FluentHelpers.BuildTypeMember(TypeMemberKind.Interface, name, body);

        public TypeMember MODULE(string name, Action body)
            => FluentHelpers.BuildTypeMember(TypeMemberKind.Module, name, body);

        public ConstructorMember CONSTRUCTOR(Action body)
            => new ConstructorGenerator(GetContextOrThrow(), body).GenerateMember();

        public MethodMember FUNCTION<TReturnType>(string name, Action body)
            => new MethodGenerator(GetContextOrThrow(), typeof(TReturnType), name, body).GenerateMember();
        
        public MethodMember FUNCTION(TypeMember returnType, string name, Action body)
            => new MethodGenerator(GetContextOrThrow(), returnType, name, body).GenerateMember();

        public MethodMember FUNCTION(string name, Action body)
            => new MethodGenerator(GetContextOrThrow(), name, body).GenerateMember();

        public MethodMember VOID(string name, Action body)
            => new MethodGenerator(GetContextOrThrow(), name, body).GenerateMember();

        public MethodMember VOID(MethodMember ancestorMethod, Action body)
            => new MethodGenerator(GetContextOrThrow(), ancestorMethod, body).GenerateMember();

        public void PROPERTY<T>(string name, Action body = null)
            => new PropertyGenerator(GetContextOrThrow(), typeof(T), name, body).GenerateMember();

        public void PROPERTY(TypeMember type, string name, Action body = null)
            => new PropertyGenerator(GetContextOrThrow(), type, name, body).GenerateMember();
    }
}
