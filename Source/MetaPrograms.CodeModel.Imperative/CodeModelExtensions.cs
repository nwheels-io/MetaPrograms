using System;
using System.Collections.Generic;
using System.Text;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative
{
    public static class CodeModelExtensions
    {
        public static TypeMember CloseType(this ImperativeCodeModel model, Type openType, params TypeMember[] typeArguments)
        {
            return model.Get<TypeMember>(openType).Get().MakeGenericType(typeArguments);
        }

        public static TypeMember Type<TClrType>(this ImperativeCodeModel model)
        {
            return model.Get<TypeMember>(typeof(TClrType));
        }
    }
}
