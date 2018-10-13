#if false

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using MetaPrograms.Expressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MetaPrograms.CSharp.Writer
{
    public static class ExpressionSyntaxBuilder
    {
        private static readonly IReadOnlyDictionary<Type, Func<AbstractExpression, ExpressionSyntax>> SyntaxFactoryMap =
            new Dictionary<Type, Func<AbstractExpression, ExpressionSyntax>> {
                [typeof(ConstantExpression)] = x => GetConstant((ConstantExpression)x)
            };
        
        public static ExpressionSyntax GetSyntax(AbstractExpression expression)
        {
            if (SyntaxFactoryMap.TryGetValue(expression.GetType(), out var factory))
            {
                return factory(expression);
            }
            
            throw new NotSupportedException($"Generating syntax tree from '{expression.GetType().Name}' is not supported.");
        }

        public static ExpressionSyntax GetConstant(ConstantExpression expression)
        {
            return LiteralExpression(
                SyntaxKind.StringLiteralExpression,
                Literal("ABC")
            )
        }

        public static SyntaxToken GetLiteral(object value)
        {
            if (value == null)
            {
                return Token(SyntaxKind.NullKeyword);
            }
            
            switch (Type.GetTypeCode(value.GetType()))
            {
                case TypeCode.Boolean:
                    return Token((bool)value ? SyntaxKind.TrueKeyword : SyntaxKind.FalseKeyword);
                case TypeCode.String:
                    return Literal((string)value);
                case TypeCode.Char:
                    return Literal((char)value);
                case TypeCode.Byte:
                    return Literal((Byte)value);
                case TypeCode.SByte:
                    return Literal((SByte)value);
                case TypeCode.Int16:
                    return Literal((Int16)value);
                case TypeCode.UInt16:
                    return Literal((UInt16)value);
                case TypeCode.Int32:
                    return Literal((Int32)value);
                case TypeCode.UInt32:
                    return Literal((UInt32)value);
                case TypeCode.Int64:
                    return Literal((Int64)value);
                case TypeCode.UInt64:
                    return Literal((UInt64)value);
                case TypeCode.Single:
                    return Literal((Single)value);
                case TypeCode.Double:
                    return Literal((Double)value);
                case TypeCode.Decimal:
                    return Literal((Decimal)value);
                default:
                    throw new NotSupportedException($"Literal syntax is not supported for value of type {value.GetType().Name}.");
            }
        }
    }
}

#endif