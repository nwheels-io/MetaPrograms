﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using MetaPrograms.CodeModel.Imperative;
using MetaPrograms.CodeModel.Imperative.Expressions;
using MetaPrograms.CodeModel.Imperative.Fluent;
using MetaPrograms.CodeModel.Imperative.Members;
using MetaPrograms.CodeModel.Imperative.Statements;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

namespace MetaPrograms.Adapters.Roslyn.Reader
{
    public class OperationReader
    {
        private static readonly IReadOnlyDictionary<OperationKind, Func<IOperation, AbstractExpression>> ExpressionReaderByOperationKind =
            new Dictionary<OperationKind, Func<IOperation, AbstractExpression>> {
                [OperationKind.SimpleAssignment] = op => ReadAssignment((IAssignmentOperation)op),
                [OperationKind.EventAssignment] = op => ReadEventAssignment((IEventAssignmentOperation)op),
                [OperationKind.InstanceReference] = op => ReadInstanceReference(op),
                [OperationKind.MethodReference] = op => ReadMethodReference((IMethodReferenceOperation)op),
                [OperationKind.PropertyReference] = op => ReadPropertyReference((IPropertyReferenceOperation)op),
                [OperationKind.EventReference] = op => ReadEventReference((IEventReferenceOperation)op),
                [OperationKind.DelegateCreation] = op => ReadDelegateCreation((IDelegateCreationOperation)op),
                [OperationKind.AnonymousFunction] = op => ReadAnonymousFunction((IAnonymousFunctionOperation)op),
                [OperationKind.Await] = op => ReadAwait((IAwaitOperation)op),
                [OperationKind.Invocation] = op => ReadInvocation((IInvocationOperation)op),
                [OperationKind.ObjectCreation] = op => ReadObjectCreation((IObjectCreationOperation)op),
            };

        private static readonly IReadOnlyDictionary<OperationKind, Func<IOperation, AbstractStatement>> StatementReaderByOperationKind =
            new Dictionary<OperationKind, Func<IOperation, AbstractStatement>> {
                [OperationKind.Block] = op => ReadBlock((IBlockOperation)op),
                [OperationKind.ExpressionStatement] = op => ReadExpressionStatement((IExpressionStatementOperation)op),
                [OperationKind.Return] = op => ReadReturn((IReturnOperation)op)
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

        private static AbstractExpression ReadAssignment(IAssignmentOperation op)
        {
            var context = CodeReaderContext.GetContextOrThrow();
            var result = new AssignmentExpression();

            result.Left = (IAssignable)ReadExpression(op.Target);

            if (op.ConstantValue.HasValue)
            {
                result.Right = context.GetConstantExpression(op.ConstantValue.Value);
            }
            else
            {
                result.Right = ReadExpression(op.Value);
            }

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

        private static AbstractExpression ReadEventReference(IEventReferenceOperation op)
        {
            var context = CodeReaderContext.GetContextOrThrow();
            
            return new MemberExpression {
                Target = ReadExpression(op.Instance),
                Member = context.FindMemberOrThrow<EventMember>(op.Event)
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

            return new NewObjectExpression {
                Type = objectType,
                ConstructorCall = new MethodCallExpression {
                    Type = objectType,
                    Method = context.CodeModel.Get<ConstructorMember>(op.Constructor),
                    Arguments = op.Arguments.Select(ReadArgument).ToList()
                }
            };
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