using System;
using MetaPrograms.Expressions;
using MetaPrograms.Members;
using Microsoft.CodeAnalysis;

namespace MetaPrograms.CSharp
{
    public static class ImperativeCodeModelExtensions
    {
        public static TypeMember TryGetMemberFromTypeof(this ImperativeCodeModel codeModel, AbstractExpression expression)
        {
            return (
                (expression as ConstantExpression)?.Value is INamedTypeSymbol typeSymbol
                ? codeModel.TryGet<TypeMember>(binding: typeSymbol)
                : null);
        }

        public static TypeMember GetMemberFromTypeof(this ImperativeCodeModel codeModel, AbstractExpression expression)
        {
            return (
                TryGetMemberFromTypeof(codeModel, expression)
                ?? throw new Exception($"could not find TypeMember referred by typeof"));
        }
    }
}
