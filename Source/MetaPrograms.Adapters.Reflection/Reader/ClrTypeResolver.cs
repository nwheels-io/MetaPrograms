using System;
using MetaPrograms.CodeModel.Imperative;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.Adapters.Reflection.Reader
{
    public class ClrTypeResolver : IClrTypeResolver
    {
        TypeMember IClrTypeResolver.Resolve(Type clrType, ImperativeCodeModel codeModel, int distance)
        {
            return ResolveType(clrType, codeModel, distance);
        }

        public static TypeMember ResolveType(Type clrType, ImperativeCodeModel codeModel, int distance)
        {
            var typeMember = new TypeMember();
            var reader = new ClrTypeReader(clrType, typeMember, codeModel, distance);
            
            codeModel.Add(typeMember, isTopLevel: !clrType.IsNested);
            
            if (distance > 0)
            {
                reader.ReadNameOnly();
            }
            else
            {
                reader.ReadAll();
            }

            return typeMember;
        }

        public void Complete(TypeMember existingType, ImperativeCodeModel codeModel)
        {
            var clrType = existingType.Bindings.First<System.Type>();
            var reader = new ClrTypeReader(clrType, existingType, codeModel, distance: 0);
            
            reader.ReadAll();
        }
    }
}
