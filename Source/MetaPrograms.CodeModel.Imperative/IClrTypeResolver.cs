using System;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative
{
    public interface IClrTypeResolver
    {
        MemberRef<TypeMember> Resolve(Type clrType, ImperativeCodeModel codeModel, int distance);
        void Complete(MemberRef<TypeMember> existingRef, ImperativeCodeModel codeModel);
    }

    public static class ClrTypeResolverExtensions
    {
        public static MemberRef<TypeMember> Resolve<TClrType>(
            this IClrTypeResolver resolver, 
            ImperativeCodeModel codeModel, 
            int distance)
        {
            return resolver.Resolve(typeof(TClrType), codeModel, distance);
        }
    }
}