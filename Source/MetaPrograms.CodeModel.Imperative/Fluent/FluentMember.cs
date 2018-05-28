using System;
using System.Collections.Immutable;
using MetaPrograms.CodeModel.Imperative.Members;

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

        public void CONSTRUCTOR(Action body) { }

        public void FUNCTION<TReturnType>(string name, Action body) { }
        public void FUNCTION(TypeMember returnType, string name) { }

        public void VOID(string name, Action body) { }
        public void VOID(MethodMember ancestorMethod, Action body) { }

        public FluentProperty PROPERTY<T>(string name, Action body = null) => null;
        public FluentProperty PROPERTY(TypeMember type, string name, Action body = null) => null;
    }
}