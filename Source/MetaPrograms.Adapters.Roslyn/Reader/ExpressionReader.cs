using System;
using System.Collections.Generic;
using System.Text;
using MetaPrograms.CodeModel.Imperative;
using MetaPrograms.CodeModel.Imperative.Expressions;
using MetaPrograms.CodeModel.Imperative.Members;
using Microsoft.CodeAnalysis;

namespace MetaPrograms.Adapters.Roslyn.Reader
{
    public static class ExpressionReader
    {
        public static ConstantExpression ReadTypedConstant(CodeModelBuilder modelBuilder, TypedConstant constant)
        {
            return new ConstantExpression(modelBuilder.TryGetMember<TypeMember>(constant.Type), constant.Value);
        }
    }
}
