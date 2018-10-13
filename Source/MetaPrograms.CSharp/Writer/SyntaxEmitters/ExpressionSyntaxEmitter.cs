using System;
using System.Collections.Generic;
using System.Linq;
using MetaPrograms.Expressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MetaPrograms.CSharp.Writer.SyntaxEmitters
{
    public static class ExpressionSyntaxEmitter
    {
        private static readonly IReadOnlyDictionary<UnaryOperator, SyntaxKind> UnarySyntaxMap =
            new Dictionary<UnaryOperator, SyntaxKind> {
                { UnaryOperator.LogicalNot, SyntaxKind.LogicalNotExpression },
                { UnaryOperator.BitwiseNot, SyntaxKind.BitwiseNotExpression },
                { UnaryOperator.Plus, SyntaxKind.UnaryPlusExpression },
                { UnaryOperator.Negation, SyntaxKind.UnaryMinusExpression },
                { UnaryOperator.PreIncrement, SyntaxKind.PreIncrementExpression },
                { UnaryOperator.PostIncrement, SyntaxKind.PostIncrementExpression },
                { UnaryOperator.PreDecrement, SyntaxKind.PreDecrementExpression },
                { UnaryOperator.PostDecrement, SyntaxKind.PostDecrementExpression }
            };

        private static readonly IReadOnlyDictionary<BinaryOperator, SyntaxKind> BinarySyntaxMap =
            new Dictionary<BinaryOperator, SyntaxKind> {
                { BinaryOperator.Add, SyntaxKind.AddExpression },
                { BinaryOperator.Subtract, SyntaxKind.SubtractExpression },
                { BinaryOperator.Multiply, SyntaxKind.MultiplyExpression },
                { BinaryOperator.Divide,  SyntaxKind.DivideExpression },
                { BinaryOperator.Modulus, SyntaxKind.ModuloExpression },
                { BinaryOperator.LogicalAnd, SyntaxKind.LogicalAndExpression },
                { BinaryOperator.LogicalOr, SyntaxKind.LogicalOrExpression },
                { BinaryOperator.LogicalXor, SyntaxKind.ExclusiveOrExpression },
                { BinaryOperator.BitwiseAnd, SyntaxKind.BitwiseAndExpression },
                { BinaryOperator.BitwiseOr, SyntaxKind.BitwiseOrExpression },
                { BinaryOperator.BitwiseXor, SyntaxKind.ExclusiveOrExpression },
                { BinaryOperator.LeftShift, SyntaxKind.LeftShiftExpression },
                { BinaryOperator.RightShift, SyntaxKind.RightShiftExpression },
                { BinaryOperator.Equal, SyntaxKind.EqualsExpression },
                { BinaryOperator.NotEqual, SyntaxKind.NotEqualsExpression },
                { BinaryOperator.GreaterThan, SyntaxKind.GreaterThanExpression },
                { BinaryOperator.LessThan, SyntaxKind.LessThanExpression },
                { BinaryOperator.GreaterThanOrEqual, SyntaxKind.GreaterThanOrEqualExpression },
                { BinaryOperator.LessThanOrEqual, SyntaxKind.LessThanOrEqualExpression },
                { BinaryOperator.NullCoalesce, SyntaxKind.CoalesceExpression },
            };

        public static ExpressionSyntax EmitSyntax(AbstractExpression expression)
        {
            switch (expression)
            {
                case null:
                    throw new ArgumentNullException(nameof(expression));

                case ConstantExpression constant:
                    return SyntaxHelpers.GetLiteralSyntax(constant.Value);

                case LocalVariableExpression local:
                    return SyntaxFactory.IdentifierName(local.Variable.Name);

                case MethodCallExpression call:
                    return EmitMethodCallSyntax(call);

                case NewObjectExpression newObject:
                    return EmitNewObjectSyntax(newObject);

                case ThisExpression @this:
                    return SyntaxFactory.ThisExpression();

                case BaseExpression @base:
                    return SyntaxFactory.BaseExpression();

                case ParameterExpression argument:
                    return SyntaxFactory.IdentifierName(argument.Parameter.Name);

                case AssignmentExpression assignment:
                    return SyntaxFactory.AssignmentExpression(
                        SyntaxKind.SimpleAssignmentExpression,
                        EmitSyntax(assignment.Left.AsExpression()),
                        EmitSyntax(assignment.Right));

                case MemberExpression member:
                    var identifierSyntax = SyntaxFactory.IdentifierName(member.Member?.Name ?? member.MemberName);

                    if (member.Target is ThisExpression)
                    {
                        return identifierSyntax;
                    }

                    return SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        EmitSyntax(member.Target),
                        identifierSyntax);

                case NewArrayExpression newArray:
                    return EmitNewArraySyntax(newArray);

                case IndexerExpression indexer:
                    return SyntaxFactory.ElementAccessExpression(EmitSyntax(indexer.Target)).WithArgumentList(
                        SyntaxFactory.BracketedArgumentList(
                            SyntaxFactory.SeparatedList<ArgumentSyntax>(
                                indexer.IndexArguments.Select(arg => SyntaxFactory.Argument(EmitSyntax(arg))))));

                case BinaryExpression binary:
                    return SyntaxFactory.BinaryExpression(GetBinaryOperatorKeyword(binary.Operator), EmitSyntax(binary.Left), EmitSyntax(binary.Right));

                case UnaryExpression unary:
                    return EmitUnaryExpression(unary);

                case AwaitExpression @await:
                    return SyntaxFactory.AwaitExpression(EmitSyntax(@await.Expression));

                //TODO: support other types of expressions

                default:
                    throw new NotSupportedException($"Syntax emitter is not supported for expression node of type '{expression.GetType().Name}'.");
            }
        }

        private static ExpressionSyntax EmitUnaryExpression(UnaryExpression unary)
        {
            if (unary.Operator.IsPrefix())
            {
                return SyntaxFactory.PrefixUnaryExpression(GetUnaryOperatorKeyword(unary.Operator), EmitSyntax(unary.Operand));
            }
            else
            {
                return SyntaxFactory.PostfixUnaryExpression(GetUnaryOperatorKeyword(unary.Operator), EmitSyntax(unary.Operand));
            }
        }

        private static ExpressionSyntax EmitNewObjectSyntax(NewObjectExpression newObject)
        {
            var syntax = SyntaxFactory.ObjectCreationExpression(newObject.Type.GetTypeNameSyntax());

            if (newObject.ConstructorCall != null)
            {
                syntax = syntax.WithArgumentList(newObject.ConstructorCall.GetArgumentListSyntax());
            }
            else
            {
                syntax = syntax.WithArgumentList(SyntaxFactory.ArgumentList());
            }

            return syntax;
        }

        private static ExpressionSyntax EmitNewArraySyntax(NewArrayExpression newArray)
        {
            SyntaxList<ArrayRankSpecifierSyntax> rankSpecifiers = (
                newArray.DimensionLengths.Count > 0
                ? SyntaxFactory.List(newArray.DimensionLengths.Select(dimLen => SyntaxFactory.ArrayRankSpecifier(SyntaxFactory.SingletonSeparatedList(EmitSyntax(dimLen)))))
                : SyntaxFactory.SingletonList(SyntaxFactory.ArrayRankSpecifier(SyntaxFactory.SingletonSeparatedList<ExpressionSyntax>(SyntaxFactory.OmittedArraySizeExpression())))
            );

            var syntax = SyntaxFactory.ArrayCreationExpression(
                SyntaxFactory.ArrayType(newArray.ElementType.GetTypeNameSyntax()).WithRankSpecifiers(rankSpecifiers)
            );

            //TODO: support multi-dimensional array initializers
            if (newArray.DimensionInitializerValues.Count > 0)
            {
                syntax = syntax.WithInitializer(
                    SyntaxFactory.InitializerExpression(
                        SyntaxKind.ArrayInitializerExpression,
                        SyntaxFactory.SeparatedList(newArray.DimensionInitializerValues[0].Select(EmitSyntax))));
            }

            return syntax;
        }

        private static ExpressionSyntax EmitMethodCallSyntax(MethodCallExpression call)
        {
            InvocationExpressionSyntax syntax;
            var methodIdentifier = SyntaxFactory.IdentifierName(call.MethodName ?? call.Method.Name);

            if (call.Target != null)
            {
                syntax = SyntaxFactory.InvocationExpression(
                    SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        EmitSyntax(call.Target),
                        methodIdentifier));
            }
            else
            {
                syntax = SyntaxFactory.InvocationExpression(methodIdentifier);
            }

            if (call.Arguments.Count > 0)
            {
                syntax = syntax.WithArgumentList(call.GetArgumentListSyntax());
            }

            return syntax;
        }

        private static SyntaxKind GetUnaryOperatorKeyword(UnaryOperator op)
        {
            if (UnarySyntaxMap.TryGetValue(op, out var keyword))
            {
                return keyword;
            }

            throw new NotSupportedException($"Unary operator '{op}' is not supported.");
        }

        private static SyntaxKind GetBinaryOperatorKeyword(BinaryOperator op)
        {
            if (BinarySyntaxMap.TryGetValue(op, out var keyword))
            {
                return keyword;
            }

            throw new NotSupportedException($"Binary operator '{op}' is not supported.");
        }
    }
}
