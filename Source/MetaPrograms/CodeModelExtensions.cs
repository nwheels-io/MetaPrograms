using System;
using System.Collections.Generic;
using System.Text;
using MetaPrograms.Members;

namespace MetaPrograms
{
    public static class CodeModelExtensions
    {
        public static TypeMember CloseType(this ImperativeCodeModel model, Type openType, params TypeMember[] typeArguments)
        {
            return model.Get<TypeMember>(openType).MakeGenericType(typeArguments);
        }

        public static TypeMember Type<TClrType>(this ImperativeCodeModel model)
        {
            return model.Get<TypeMember>(typeof(TClrType));
        }
    }
}
