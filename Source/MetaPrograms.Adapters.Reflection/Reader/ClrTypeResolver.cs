using System;
using MetaPrograms.CodeModel.Imperative;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.Adapters.Reflection.Reader
{
    public class ClrTypeResolver : IClrTypeResolver
    {
        MemberRef<TypeMember> IClrTypeResolver.Resolve(Type clrType, ImperativeCodeModel codeModel, int distance)
        {
            return ResolveType(clrType, codeModel, distance);
        }

        public static MemberRef<TypeMember> ResolveType(Type clrType, ImperativeCodeModel codeModel, int distance)
        {
            var builder = new TypeMemberBuilder();
            var reader = new ClrTypeReader(clrType, builder, codeModel, distance);
            
            codeModel.Add(builder.GetTemporaryProxy().GetRef(), isTopLevel: !clrType.IsNested);
            
            if (distance > 0)
            {
                reader.ReadNameOnly();
            }
            else
            {
                reader.ReadAll();
            }

            var finalMember = new RealTypeMember(builder);
            builder.GetMemberSelfReference().Reassign(finalMember);

            return finalMember.GetRef();
        }

        public void Complete(MemberRef<TypeMember> existingRef, ImperativeCodeModel codeModel)
        {
            var clrType = existingRef.Get().Bindings.First<System.Type>();
            var builder = existingRef.Get().CreateCompletionBuilder();
            var reader = new ClrTypeReader(clrType, builder, codeModel, distance: 0);
            
            reader.ReadAll();

            var finalMember = new RealTypeMember(builder);
            builder.GetMemberSelfReference().Reassign(finalMember);
        }
    }
}
