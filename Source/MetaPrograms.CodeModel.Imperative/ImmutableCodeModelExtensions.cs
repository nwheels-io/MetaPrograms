using System;
using System.Collections.Generic;
using System.Text;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative
{
    public static class ImmutableCodeModelExtensions
    {
        public static TypeMember CloseType(this ImmutableCodeModel model, Type openType, params TypeMember[] typeArguments)
        {
            return model.Get<TypeMember>(openType).MakeGenericType(typeArguments);
        }

        public static TypeMember Type<TClrType>(this ImmutableCodeModel model)
        {
            return model.Get<TypeMember>(typeof(TClrType));
        }
    }
}
