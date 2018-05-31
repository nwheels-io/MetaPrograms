#if false

using System;
using System.Collections.Generic;
using MetaPrograms.CodeModel.Imperative;
using MetaPrograms.CodeModel.Imperative.Expressions;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.Adapters.Reflection.Reader
{
    public class ClrTypeReaderResolver
    {
        private readonly CodeModel.Imperative.ImperativeCodeModel _baseModel;
        private readonly Dictionary<Type, MemberRef<TypeMember>> _typeMemberByClrType = new Dictionary<Type, MemberRef<TypeMember>>();

        public ClrTypeReaderResolver(CodeModel.Imperative.ImperativeCodeModel baseModel)
        {
            _baseModel = baseModel;
        }

        public MemberRef<TypeMember> GetType(Type clrType, int distance)
        {
            //TODO: handle case of initially incomplete type, which is required later in full
            
            if (_baseModel.MembersByBndings.TryGetValue(clrType, out var member) && member is TypeMember type)
            {
                return type.GetRef();
            }
            
            if (_typeMemberByClrType.TryGetValue(clrType, out var registeredMember))
            {
                return registeredMember;
            }

            return ReadAndRegisterMember(clrType, distance).GetRef();
        }

        public AbstractExpression GetConstantExpression(object value)
        {
            return AbstractExpression.FromValue(value, resolveType: t => GetType(t, distance: 0));
        }

        public IReadOnlyDictionary<Type, MemberRef<TypeMember>> TypeMemberByClrType => _typeMemberByClrType;

        private TypeMember ReadAndRegisterMember(Type clrType, int distance)
        {
            var builder = new TypeMemberBuilder();
            var reader = new ClrTypeReader(clrType, builder, resolver: this, distance);

            _typeMemberByClrType.Add(clrType, builder.GetTemporaryProxy().GetRef());

            if (distance == 0)
            {
                reader.ReadAll();
            }
            else
            {
                reader.ReadNameOnly();
            }

            var finalMember = new RealTypeMember(builder);
            builder.GetMemberSelfReference().Reassign(finalMember);

            return finalMember;
        }
    }
}

#endif