using System;
using MetaPrograms.Members;

namespace MetaPrograms
{
    public interface IClrTypeResolver
    {
        TypeMember Resolve(Type clrType, ImperativeCodeModel codeModel, int distance);
        void Complete(TypeMember existingType, ImperativeCodeModel codeModel);
    }

    public static class ClrTypeResolverExtensions
    {
        public static TypeMember Resolve<TClrType>(
            this IClrTypeResolver resolver, 
            ImperativeCodeModel codeModel, 
            int distance)
        {
            return resolver.Resolve(typeof(TClrType), codeModel, distance);
        }
    }
}