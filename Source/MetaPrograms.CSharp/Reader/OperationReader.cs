using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using MetaPrograms;
using MetaPrograms.Expressions;
using MetaPrograms.Fluent;
using MetaPrograms.Members;
using MetaPrograms.Statements;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

namespace MetaPrograms.CSharp.Reader
{
    public class OperationReader
    {
        private static readonly IReadOnlyDictionary<OperationKind, Func<IOperation, AbstractExpression>> ExpressionReaderByOperationKind =
            new Dictionary<OperationKind, Func<IOperation, AbstractExpression>> {
                [OperationKind.Literal] = op => ReadLiteral((ILiteralOperation)op),
                [OperationKind.SimpleAssignment] = op => ReadAssignment((IAssignmentOperation)op),
                [OperationKind.EventAssignment] = op => ReadEventAssignment((IEventAssignmentOperation)op),
                [OperationKind.InstanceReference] = op => ReadInstanceReference(op),
                [OperationKind.MethodReference] = op => ReadMethodReference((IMethodReferenceOperation)op),
                [OperationKind.PropertyReference] = op => ReadPropertyReference((IPropertyReferenceOperation)op),
                [OperationKind.FieldReference] = op => ReadFieldReference((IFieldReferenceOperation)op),
                [OperationKind.EventReference] = op => ReadEventReference((IEventReferenceOperation)op),
                [OperationKind.ParameterReference] = op => ReadParameterReference((IParameterReferenceOperation)op),
                [OperationKind.DelegateCreation] = op => ReadDelegateCreation((IDelegateCreationOperation)op),
                [OperationKind.AnonymousFunction] = op => ReadAnonymousFunction((IAnonymousFunctionOperation)op),
                [OperationKind.Await] = op => ReadAwait((IAwaitOperation)op),
                [OperationKind.Invocation] = op => ReadInvocation((IInvocationOperation)op),
                [OperationKind.ObjectCreation] = op => ReadObjectCreation((IObjectCreationOperation)op),
                [OperationKind.ArrayCreation] = op => ReadArrayCreation((IArrayCreationOperation)op),
                [OperationKind.ObjectOrCollectionInitializer] = op => ReadObjectOrCollectionInitializer((IObjectOrCollectionInitializerOperation)op),
                [OperationKind.Conditional] = op => ReadConditional((IConditionalOperation)op),
                [OperationKind.BinaryOperator] = op => ReadBinaryOperator((IBinaryOperation)op),
                [OperationKind.Conversion] = op => ReadConversion((IConversionOperation)op),
                [OperationKind.InterpolatedString] = op => ReadInterpolatedString((IInterpolatedStringOperation)op),
                [OperationKind.TypeOf] = op => ReadTypeOf((ITypeOfOperation)op)
            };

        private static readonly IReadOnlyDictionary<OperationKind, Func<IOperation, AbstractStatement>> StatementReaderByOperationKind =
            new Dictionary<OperationKind, Func<IOperation, AbstractStatement>> {
                [OperationKind.Block] = op => ReadBlock((IBlockOperation)op),
                [OperationKind.ExpressionStatement] = op => ReadExpressionStatement((IExpressionStatementOperation)op),
                [OperationKind.Return] = op => ReadReturn((IReturnOperation)op)
            };

        private static readonly IReadOnlyDictionary<BinaryOperatorKind, BinaryOperator> BinaryOperatorByOperatorKind =
            new Dictionary<BinaryOperatorKind, BinaryOperator>() {
                { BinaryOperatorKind.Add, BinaryOperator.Add },
                { BinaryOperatorKind.Subtract, BinaryOperator.Subtract },
                { BinaryOperatorKind.Multiply, BinaryOperator.Multiply },
                { BinaryOperatorKind.Divide, BinaryOperator.Divide },
                { BinaryOperatorKind.IntegerDivide, BinaryOperator.Divide },
                { BinaryOperatorKind.Remainder, BinaryOperator.Modulus },
                { BinaryOperatorKind.LeftShift, BinaryOperator.LeftShift },
                { BinaryOperatorKind.RightShift, BinaryOperator.RightShift },
                { BinaryOperatorKind.And, BinaryOperator.BitwiseAnd },
                { BinaryOperatorKind.Or, BinaryOperator.BitwiseOr },
                { BinaryOperatorKind.ExclusiveOr, BinaryOperator.BitwiseXor },
                { BinaryOperatorKind.ConditionalAnd, BinaryOperator.LogicalAnd },
                { BinaryOperatorKind.ConditionalOr, BinaryOperator.LogicalOr },
                { BinaryOperatorKind.Equals, BinaryOperator.Equal },
                { BinaryOperatorKind.NotEquals, BinaryOperator.NotEqual },
                { BinaryOperatorKind.LessThan, BinaryOperator.LessThan },
                { BinaryOperatorKind.LessThanOrEqual, BinaryOperator.LessThanOrEqual },
                { BinaryOperatorKind.GreaterThanOrEqual, BinaryOperator.GreaterThanOrEqual },
                { BinaryOperatorKind.GreaterThan, BinaryOperator.GreaterThan }
            };

        private readonly CodeModelBuilder _codeModel;
        private readonly IFunctionContext _destination;
        private readonly SyntaxNode _syntax;
        private readonly SemanticModel _semanticModel;

        public OperationReader(CodeModelBuilder codeModel, IFunctionContext destination, SyntaxNode syntax)
        {
            _codeModel = codeModel;
            _destination = destination;
            _syntax = syntax;
            _semanticModel = _codeModel.GetSemanticModel(syntax);
        }

        public void ReadMethodBody()
        {
            var rootOperation = _semanticModel.GetOperation(_syntax);

            if (rootOperation is IBlockOperation)
            {
                _destination.Body = (BlockStatement)ReadStatement(rootOperation);
            }
            else if (rootOperation is IMethodBodyBaseOperation methodBodyOperation)
            {
                _destination.Body = (BlockStatement)ReadStatement(methodBodyOperation.ExpressionBody ?? methodBodyOperation.BlockBody);
            }
            else
            {
                throw new NotSupportedException(
                    $"Statement '{rootOperation.GetType().Name}' cannot be read as ${nameof(BlockStatement)}.");
            }
        }

        private static AbstractStatement ReadStatement(IOperation operation)
        {
            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            if (StatementReaderByOperationKind.TryGetValue(operation.Kind, out var reader))
            {
                return reader(operation);
            }
            
            throw new NotSupportedException($"Operation kind {operation.Kind} (type {operation.GetType().Name}) is not supported.");
        }
        
        private static AbstractExpression ReadExpression(IOperation operation)
        {
            if (operation == null)
            {
                return null;
            }

            if (ExpressionReaderByOperationKind.TryGetValue(operation.Kind, out var reader))
            {
                return reader(operation);
            }
            
            throw new NotSupportedException($"Operation kind {operation.Kind} (type {operation.GetType().Name}) is not supported.");
        }

        private static AbstractExpression ReadAssignmentValue(IAssignmentOperation op)
        {
            var context = CodeReaderContext.GetContextOrThrow();

            if (op.ConstantValue.HasValue)
            {
                return context.GetConstantExpression(op.ConstantValue.Value);
            }
            else
            {
                return ReadExpression(op.Value);
            }
        }

        /*
        Microsoft.CodeAnalysis.Operations. IAddressOfOperation
        Microsoft.CodeAnalysis.Operations. IAnonymousFunctionOperation
        Microsoft.CodeAnalysis.Operations. IAnonymousObjectCreationOperation
        Microsoft.CodeAnalysis.Operations. IArgumentOperation
        Microsoft.CodeAnalysis.Operations. IArrayCreationOperation
        Microsoft.CodeAnalysis.Operations. IArrayElementReferenceOperation
        Microsoft.CodeAnalysis.Operations. IArrayInitializerOperation
        - Microsoft.CodeAnalysis.Operations. IAssignmentOperation
        Microsoft.CodeAnalysis.Operations. IAwaitOperation
        Microsoft.CodeAnalysis.Operations. IBinaryOperation
        - Microsoft.CodeAnalysis.Operations. IBlockOperation
        Microsoft.CodeAnalysis.Operations. IBranchOperation
        Microsoft.CodeAnalysis.Operations. ICaseClauseOperation
        Microsoft.CodeAnalysis.Operations. ICatchClauseOperation
        Microsoft.CodeAnalysis.Operations. ICoalesceOperation
        Microsoft.CodeAnalysis.Operations. ICollectionElementInitializerOperation
        Microsoft.CodeAnalysis.Operations. ICompoundAssignmentOperation
        Microsoft.CodeAnalysis.Operations. IConditionalAccessInstanceOperation
        Microsoft.CodeAnalysis.Operations. IConditionalAccessOperation
        Microsoft.CodeAnalysis.Operations. IConditionalOperation
        Microsoft.CodeAnalysis.Operations. IConstantPatternOperation
        Microsoft.CodeAnalysis.Operations. IConversionOperation
        Microsoft.CodeAnalysis.Operations. IDeclarationExpressionOperation
        Microsoft.CodeAnalysis.Operations. IDeclarationPatternOperation
        Microsoft.CodeAnalysis.Operations. IDeconstructionAssignmentOperation
        Microsoft.CodeAnalysis.Operations. IDefaultCaseClauseOperation
        Microsoft.CodeAnalysis.Operations. IDefaultValueOperation
        Microsoft.CodeAnalysis.Operations. IDelegateCreationOperation
        Microsoft.CodeAnalysis.Operations. IDynamicIndexerAccessOperation
        Microsoft.CodeAnalysis.Operations. IDynamicInvocationOperation
        Microsoft.CodeAnalysis.Operations. IDynamicMemberReferenceOperation
        Microsoft.CodeAnalysis.Operations. IDynamicObjectCreationOperation
        Microsoft.CodeAnalysis.Operations. IEmptyOperation
        Microsoft.CodeAnalysis.Operations. IEndOperation
        Microsoft.CodeAnalysis.Operations. IEventAssignmentOperation
        Microsoft.CodeAnalysis.Operations. IEventReferenceOperation
        Microsoft.CodeAnalysis.Operations. IExpressionStatementOperation
        Microsoft.CodeAnalysis.Operations. IFieldInitializerOperation
        Microsoft.CodeAnalysis.Operations. IFieldReferenceOperation
        Microsoft.CodeAnalysis.Operations. IFixedOperation
        Microsoft.CodeAnalysis.Operations. IForEachLoopOperation
        Microsoft.CodeAnalysis.Operations. IForLoopOperation
        Microsoft.CodeAnalysis.Operations. IForToLoopOperation
        Microsoft.CodeAnalysis.Operations. IIncrementOrDecrementOperation
        Microsoft.CodeAnalysis.Operations. IInstanceReferenceOperation
        Microsoft.CodeAnalysis.Operations. IInterpolatedStringContentOperation
        Microsoft.CodeAnalysis.Operations. IInterpolatedStringOperation
        Microsoft.CodeAnalysis.Operations. IInterpolatedStringTextOperation
        Microsoft.CodeAnalysis.Operations. IInterpolationOperation
        Microsoft.CodeAnalysis.Operations. IInvalidOperation
        Microsoft.CodeAnalysis.Operations. IInvocationOperation
        Microsoft.CodeAnalysis.Operations. IIsPatternOperation
        Microsoft.CodeAnalysis.Operations. IIsTypeOperation
        Microsoft.CodeAnalysis.Operations. ILabeledOperation
        Microsoft.CodeAnalysis.Operations. ILiteralOperation
        Microsoft.CodeAnalysis.Operations. ILocalFunctionOperation
        Microsoft.CodeAnalysis.Operations. ILocalReferenceOperation
        Microsoft.CodeAnalysis.Operations. ILockOperation
        Microsoft.CodeAnalysis.Operations. ILoopOperation
        Microsoft.CodeAnalysis.Operations. IMemberInitializerOperation
        Microsoft.CodeAnalysis.Operations. IMemberReferenceOperation
        Microsoft.CodeAnalysis.Operations. IMethodReferenceOperation
        Microsoft.CodeAnalysis.Operations. INameOfOperation
        Microsoft.CodeAnalysis.Operations. IObjectCreationOperation
        Microsoft.CodeAnalysis.Operations. IObjectOrCollectionInitializerOperation
        Microsoft.CodeAnalysis.Operations. IOmittedArgumentOperation
        Microsoft.CodeAnalysis.Operations. IParameterInitializerOperation
        Microsoft.CodeAnalysis.Operations. IParameterReferenceOperation
        Microsoft.CodeAnalysis.Operations. IParenthesizedOperation
        Microsoft.CodeAnalysis.Operations. IPatternCaseClauseOperation
        Microsoft.CodeAnalysis.Operations. IPatternOperation
        Microsoft.CodeAnalysis.Operations. IPlaceholderOperation
        Microsoft.CodeAnalysis.Operations. IPointerIndirectionReferenceOperation
        Microsoft.CodeAnalysis.Operations. IPropertyInitializerOperation
        Microsoft.CodeAnalysis.Operations. IPropertyReferenceOperation
        Microsoft.CodeAnalysis.Operations. IRaiseEventOperation
        Microsoft.CodeAnalysis.Operations. IRangeCaseClauseOperation
        Microsoft.CodeAnalysis.Operations. IRelationalCaseClauseOperation
        Microsoft.CodeAnalysis.Operations. IReturnOperation
        Microsoft.CodeAnalysis.Operations. ISimpleAssignmentOperation
        Microsoft.CodeAnalysis.Operations. ISingleValueCaseClauseOperation
        Microsoft.CodeAnalysis.Operations. ISizeOfOperation
        Microsoft.CodeAnalysis.Operations. IStopOperation
        Microsoft.CodeAnalysis.Operations. ISwitchCaseOperation
        Microsoft.CodeAnalysis.Operations. ISwitchOperation
        Microsoft.CodeAnalysis.Operations. ISymbolInitializerOperation
        Microsoft.CodeAnalysis.Operations. IThrowOperation
        Microsoft.CodeAnalysis.Operations. ITranslatedQueryOperation
        Microsoft.CodeAnalysis.Operations. ITryOperation
        Microsoft.CodeAnalysis.Operations. ITupleOperation
        Microsoft.CodeAnalysis.Operations. ITypeOfOperation
        Microsoft.CodeAnalysis.Operations. ITypeParameterObjectCreationOperation
        Microsoft.CodeAnalysis.Operations. IUnaryOperation
        Microsoft.CodeAnalysis.Operations. IUsingOperation
        Microsoft.CodeAnalysis.Operations. IVariableDeclarationGroupOperation
        Microsoft.CodeAnalysis.Operations. IVariableDeclarationOperation
        Microsoft.CodeAnalysis.Operations. IVariableDeclaratorOperation
        Microsoft.CodeAnalysis.Operations. IVariableInitializerOperation
        Microsoft.CodeAnalysis.Operations. IWhileLoopOperation
        Microsoft.CodeAnalysis.Operations. IWithOperation
        */

        private static AbstractStatement ReadBlock(IBlockOperation op)
        {
            var context = CodeReaderContext.GetContextOrThrow();
            var result = new BlockStatement();

            using (context.PushState(new BlockContext(result)))
            {
                foreach (var childOp in op.Operations)
                {
                    if (!IsRedundantStatement(childOp))
                    {
                        result.Statements.Add(ReadStatement(childOp));
                    }
                }
            }

            return result;
        }

        private static bool IsRedundantStatement(IOperation op)
        {
            if (op is IReturnOperation returnOp)
            {
                if (returnOp.ReturnedValue == null && 
                    !returnOp.ConstantValue.HasValue && 
                    op.Parent?.Kind == OperationKind.Block && 
                    op.Parent?.Parent?.Kind == OperationKind.AnonymousFunction)
                {
                    return true;
                }
            }

            return false;
        }

        private static AbstractStatement ReadExpressionStatement(IExpressionStatementOperation op)
        {
            return new ExpressionStatement {
                Expression = ReadExpression(op.Operation)
            };
        }

        private static AbstractStatement ReadReturn(IReturnOperation op)
        {
            return new ReturnStatement {
                Expression = ReadExpression(op.ReturnedValue)
            };
        }

        private static AbstractExpression ReadLiteral(ILiteralOperation op)
        {
            var context = CodeReaderContext.GetContextOrThrow();
            var type = context.FindMemberOrThrow<TypeMember>(binding: op.Type);

            return new ConstantExpression {
                Type = type,
                Value = op.ConstantValue.HasValue ? op.ConstantValue.Value : null
            };
        }

        private static AbstractExpression ReadAssignment(IAssignmentOperation op)
        {
            var result = new AssignmentExpression();

            result.Left = (IAssignable)ReadExpression(op.Target);
            result.Right = ReadAssignmentValue(op);

            return result;
        }
        
        private static AbstractExpression ReadEventAssignment(IEventAssignmentOperation op)
        {
            return new AssignmentExpression {
                Left = (IAssignable)ReadExpression(op.EventReference),
                Right = ReadExpression(op.HandlerValue),
                CompoundOperator = (op.Adds ? CompoundAssignmentOperator.Addition : CompoundAssignmentOperator.Subtraction)
            };
        }
        
        private static AbstractExpression ReadInstanceReference(IOperation op)
        {
            return new ThisExpression();
        }

        private static AbstractExpression ReadMethodReference(IMethodReferenceOperation op)
        {
            throw new NotImplementedException();
        }

        private static AbstractExpression ReadPropertyReference(IPropertyReferenceOperation op)
        {
            var context = CodeReaderContext.GetContextOrThrow();

            return new MemberExpression {
                Target = ReadExpression(op.Instance),
                Member = context.FindMemberOrThrow<PropertyMember>(op.Property)
            };
        }

        private static AbstractExpression ReadFieldReference(IFieldReferenceOperation op)
        {
            var context = CodeReaderContext.GetContextOrThrow();

            return new MemberExpression {
                Target = ReadExpression(op.Instance),
                Member = context.FindMemberOrThrow<FieldMember>(op.Field)
            };
        }

        private static AbstractExpression ReadEventReference(IEventReferenceOperation op)
        {
            var context = CodeReaderContext.GetContextOrThrow();
            
            return new MemberExpression {
                Target = ReadExpression(op.Instance),
                Member = context.FindMemberOrThrow<EventMember>(op.Event)
            };
        }
        
        private static AbstractExpression ReadParameterReference(IParameterReferenceOperation op)
        {
            var context = CodeReaderContext.GetContextOrThrow();
            
            return new ParameterExpression() {
                // TODO: implement properly
            };
        }

        private static AbstractExpression ReadDelegateCreation(IDelegateCreationOperation op)
        {
            return ReadExpression(op.Target);
        }
        
        private static AbstractExpression ReadAnonymousFunction(IAnonymousFunctionOperation op)
        {
            var context = CodeReaderContext.GetContextOrThrow();

            return new AnonymousDelegateExpression {
                Body = (BlockStatement)ReadBlock(op.Body),
                Signature = MethodReaderMechanism.ReadSignature(context.CodeModel, op.Symbol)
            };
        }

        private static AbstractExpression ReadAwait(IAwaitOperation op)
        {
            return new AwaitExpression {
                Expression = ReadExpression(op.Operation)
            };
        }

        private static AbstractExpression ReadInvocation(IInvocationOperation op)
        {
            var context = CodeReaderContext.GetContextOrThrow();

            return new MethodCallExpression {
                Target = ReadExpression(op.Instance),
                Method = FindMethodMember(context, op.TargetMethod),
                Arguments = op.Arguments.Select(ReadArgument).ToList()
            };
        }

        private static AbstractExpression ReadObjectCreation(IObjectCreationOperation op)
        {
            var context = CodeReaderContext.GetContextOrThrow();
            var objectType = context.CodeModel.Get<TypeMember>(op.Type);
            var initializer = ReadExpression(op.Initializer);
            
            return new NewObjectExpression {
                Type = objectType,
                ConstructorCall = new MethodCallExpression {
                    Type = objectType,
                    Method = context.CodeModel.Get<ConstructorMember>(op.Constructor),
                    Arguments = op.Arguments.Select(ReadArgument).ToList()
                },
                ObjectInitializer = initializer as ObjectInitializerExpression,
                CollectionInitializer = initializer as CollectionInitializerExpression
            };
        }

        private static AbstractExpression ReadArrayCreation(IArrayCreationOperation op)
        {
            var context = CodeReaderContext.GetContextOrThrow();
            //TODO: implement properly
            return new NewArrayExpression();
        }

        private static AbstractExpression ReadObjectOrCollectionInitializer(IObjectOrCollectionInitializerOperation op)
        {
            if (IsObjectInitializer(op))
            {
                return ReadObjectInitializer(op);
            }

            return ReadCollectionInitializer(op);
        }

        private static bool IsObjectInitializer(IObjectOrCollectionInitializerOperation op)
        {
            return op.Initializers.All(init => init is IAssignmentOperation);
        }

        private static AbstractExpression ReadObjectInitializer(IObjectOrCollectionInitializerOperation op)
        {
            var context = CodeReaderContext.GetContextOrThrow();
            var result = new ObjectInitializerExpression {
                PropertyValues = op.Initializers.Select(readInitializer).ToList()
            };

            return result;

            NamedPropertyValue readInitializer(IOperation initOp)
            {
                if (initOp is IAssignmentOperation assignment &&
                    assignment.Target is IMemberReferenceOperation memberRef)
                {
                    var property = context.FindMemberOrThrow<AbstractMember>(memberRef.Member);
                    var value = ReadAssignmentValue(assignment);

                    return new NamedPropertyValue(property.Name, value);
                }

                throw new CodeReadErrorException($"Unrecognized object initializer: {initOp.Syntax}");
            }
        }

        private static AbstractExpression ReadCollectionInitializer(IObjectOrCollectionInitializerOperation op)
        {
            var context = CodeReaderContext.GetContextOrThrow();
            var result = new CollectionInitializerExpression {
                Items = op.Initializers.Select(readInitializer).ToList()
            };

            return result;

            CollectionInitializerExpression.ItemInitializer readInitializer(IOperation initOp)
            {
                if (initOp is IInvocationOperation invocation)
                {
                    var item = new CollectionInitializerExpression.ItemInitializer();
                    item.ItemArguments.AddRange(invocation.Arguments.Select(arg => ReadExpression(arg.Value)));
                    return item;
                }

                throw new CodeReadErrorException($"Unrecognized collection initializer: {initOp.Syntax}");
            }
        }

        private static AbstractExpression ReadConditional(IConditionalOperation op)
        {
            return new ConditionalExpression {
                Condition = ReadExpression(op.Condition),
                WhenTrue = ReadExpression(op.WhenTrue),
                WhenFalse = ReadExpression(op.WhenFalse)
            };
        }

        private static AbstractExpression ReadBinaryOperator(IBinaryOperation op)
        {
            var context = CodeReaderContext.GetContextOrThrow();

            return new BinaryExpression {
                Left = ReadExpression(op.LeftOperand),
                Right = ReadExpression(op.RightOperand),
                Type =  context.CodeModel.TryGet<TypeMember>(op.Type),
                Operator = BinaryOperatorByOperatorKind[op.OperatorKind]
            };
        }

        private static AbstractExpression ReadConversion(IConversionOperation op)
        {
            //TODO: enrich AbstractExpression with conversion info
            return ReadExpression(op.Operand);
        }

        private static AbstractExpression ReadInterpolatedString(IInterpolatedStringOperation op)
        {
            var result = new InterpolatedStringExpression();

            foreach (var part in op.Parts)
            {
                switch (part)
                {
                    case IInterpolatedStringTextOperation text:
                        result.Parts.Add(new InterpolatedStringExpression.TextPart {
                            Text = ReadExpression(text.Text)
                        });
                        break;
                    case IInterpolationOperation interpolation:
                        result.Parts.Add(new InterpolatedStringExpression.InterpolationPart {
                            Value = ReadExpression(interpolation.Expression),
                            Alignment = ReadExpression(interpolation.Alignment),
                            FormatString = ReadExpression(interpolation.FormatString)
                        });
                        break;
                }
            }

            return result;
        }

        private static AbstractExpression ReadTypeOf(ITypeOfOperation op)
        {
            var context = CodeReaderContext.GetContextOrThrow();
            var operand = context.CodeModel.Get<TypeMember>(op.TypeOperand);

            return new TypeReferenceExpression(operand);
        }

        private static Argument ReadArgument(IArgumentOperation op)
        {
            return new Argument {
                Modifier = op.Parameter.GetParameterModifier(),
                Expression = ReadExpression(op.Value)
            };
        }

        private static MethodMember FindMethodMember(CodeReaderContext context, IMethodSymbol symbol)
        {
            var member = context.CodeModel.TryGet<MethodMember>(symbol);
            if (member != null)
            {
                return member;
            }

            if (symbol.ConstructedFrom != null)
            {
                return context.CodeModel.Get<MethodMember>(symbol.ConstructedFrom);
            }

            throw new InvalidCodeModelException($"Method member '{symbol}' cannot be found in code model.");
        }
    }
}
