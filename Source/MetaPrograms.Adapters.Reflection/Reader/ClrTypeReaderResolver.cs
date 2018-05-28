using System;
using System.Collections.Generic;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.Adapters.Reflection.Reader
{
    public class ClrTypeReaderResolver
    {
        private readonly Dictionary<Type, MemberRef<TypeMember>> _typeMemberByClrType = new Dictionary<Type, MemberRef<TypeMember>>();

        public MemberRef<TypeMember> GetType(Type clrType)
        {
            if (_typeMemberByClrType.TryGetValue(clrType, out var registeredMember))
            {
                return registeredMember;
            }

            return ReadAndRegisterMember(clrType).GetRef();
        }

        public IReadOnlyDictionary<Type, MemberRef<TypeMember>> TypeMemberByClrType => _typeMemberByClrType;

        private TypeMember ReadAndRegisterMember(Type clrType)
        {
            var builder = new TypeMemberBuilder();
            var reader = new ClrTypeReader(clrType, builder, resolver: this);
            
            _typeMemberByClrType.Add(clrType, builder.GetTemporaryProxy().GetRef());
            
            reader.Read();
            
            var finalMember = new RealTypeMember(builder);
            builder.GetMemberSelfReference().Reassign(finalMember);

            return finalMember;
        }
    }
}