using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MetaPrograms.CodeModel.Imperative.Expressions;
using MetaPrograms.CodeModel.Imperative.Members;
using NWheels.Compilation.Adapters.Roslyn.SyntaxEmitters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace NWheels.Compilation.Adapters.Roslyn.SyntaxEmitters
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
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }
            if (expression is ConstantExpression constant)
            {
                return SyntaxHelpers.GetLiteralSyntax(constant.Value);
            }
            if (expression is LocalVariableExpression local)
            {
                return IdentifierName(local.Variable.Name);
            }
            if (expression is MethodCallExpression call)
            {
                return EmitMethodCallSyntax(call);
            }
            if (expression is NewObjectExpression newObject)
            {
                return EmitNewObjectSyntax(newObject);
            }
            if (expression is ThisExpression)
            {
                return ThisExpression();
            }
            if (expression is BaseExpression)
            {
                return BaseExpression();
            }
            if (expression is ParameterExpression argument)
            {
                return IdentifierName(argument.Parameter.Name);
            }
            if (expression is AssignmentExpression assignment)
            {
                return AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    EmitSyntax(assignment.Left.AsExpression()),
                    EmitSyntax(assignment.Right));
            }
            if (expression is MemberExpression member)
            {
                return MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    EmitSyntax(member.Target),
                    IdentifierName(member.Member?.Name ?? member.MemberName));
            }
            if (expression is NewArrayExpression newArray)
            {
                return EmitNewArraySyntax(newArray);
            }
            if (expression is IndexerExpression indexer)
            {
                return ElementAccessExpression(EmitSyntax(indexer.Target)).WithArgumentList(
                    BracketedArgumentList(
                        SeparatedList<ArgumentSyntax>(
                            indexer.IndexArguments.Select(arg => Argument(EmitSyntax(arg))))));
            }
            if (expression is BinaryExpression binary)
            {
                return BinaryExpression(GetBinaryOperatorKeyword(binary.Operator), EmitSyntax(binary.Left), EmitSyntax(binary.Right));
            }
            if (expression is UnaryExpression unary)
            {
                return EmitUnaryExpression(unary);
            }
            if (expression is AwaitExpression await)
            {
                return AwaitExpression(EmitSyntax(await.Expression));
            }

            //TODO: support other types of expressions

            throw new NotSupportedException($"Syntax emitter is not supported for expression node of type '{expression.GetType().Name}'.");
        }

        private static ExpressionSyntax EmitUnaryExpression(UnaryExpression unary)
        {
            if (unary.Operator.IsPrefix())
            {
                return PrefixUnaryExpression(GetUnaryOperatorKeyword(unary.Operator), EmitSyntax(unary.Operand));
            }
            else
            {
                return PostfixUnaryExpression(GetUnaryOperatorKeyword(unary.Operator), EmitSyntax(unary.Operand));
            }
        }

        private static ExpressionSyntax EmitNewObjectSyntax(NewObjectExpression newObject)
        {
            var syntax = ObjectCreationExpression(newObject.Type.GetTypeNameSyntax());

            if (newObject.ConstructorCall != null)
            {
                syntax = syntax.WithArgumentList(newObject.ConstructorCall.GetArgumentListSyntax());
            }
            else
            {
                syntax = syntax.WithArgumentList(ArgumentList());
            }

            return syntax;
        }

        private static ExpressionSyntax EmitNewArraySyntax(NewArrayExpression newArray)
        {
            SyntaxList<ArrayRankSpecifierSyntax> rankSpecifiers = (
                newArray.DimensionLengths.Count > 0
                ? List(newArray.DimensionLengths.Select(dimLen => ArrayRankSpecifier(SingletonSeparatedList(EmitSyntax(dimLen)))))
                : SingletonList(ArrayRankSpecifier(SingletonSeparatedList<ExpressionSyntax>(OmittedArraySizeExpression())))
            );

            var syntax = ArrayCreationExpression(
                ArrayType(newArray.ElementType.GetTypeNameSyntax()).WithRankSpecifiers(rankSpecifiers)
            );

            //TODO: support multi-dimensional array initializers
            if (newArray.DimensionInitializerValues.Count > 0)
            {
                syntax = syntax.WithInitializer(
                    InitializerExpression(
                        SyntaxKind.ArrayInitializerExpression,
                        SeparatedList(newArray.DimensionInitializerValues[0].Select(EmitSyntax))));
            }

            return syntax;
        }

        private static ExpressionSyntax EmitMethodCallSyntax(MethodCallExpression call)
        {
            InvocationExpressionSyntax syntax;
            var methodIdentifier = IdentifierName(call.MethodName ?? call.Method.Name);

            if (call.Target != null)
            {
                syntax = InvocationExpression(
                    MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        EmitSyntax(call.Target),
                        methodIdentifier));
            }
            else
            {
                syntax = InvocationExpression(methodIdentifier);
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
