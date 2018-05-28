using System;
using MetaPrograms.CodeModel.Imperative.Expressions;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Fluent
{
    public static  class FluentHelpers
    {
        public static TypeMember BuildTypeMember(TypeMemberKind typeKind, string name, Action body)
        {
            var context = CodeGeneratorContext.GetContextOrThrow();
            var traits = context.PopStateOrThrow<MemberTraitsContext>();
            var namespaceContext = context.TryLookupState<NamespaceContext>();
            var containingTypeRef = context.TryLookupState<MemberRef<TypeMember>>();

            var builder = new TypeMemberBuilder();
            builder.Namespace = namespaceContext?.Name;
            builder.Name = name;
            builder.TypeKind = typeKind;
            builder.DeclaringType = containingTypeRef;

            using (context.PushState(builder))
            {
                body?.Invoke();
            }

            var finalMember = new RealTypeMember(builder);
            builder.GetMemberSelfReference().Reassign(finalMember);
            return finalMember;
        }
    }
}