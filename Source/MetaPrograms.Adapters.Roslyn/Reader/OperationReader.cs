using System;
using System.Collections.Generic;
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
                [OperationKind.SimpleAssignment] = op => ReadAssignment((IAssignmentOperation)op)
            };

        private static readonly IReadOnlyDictionary<OperationKind, Func<IOperation, AbstractStatement>> StatementReaderByOperationKind =
            new Dictionary<OperationKind, Func<IOperation, AbstractStatement>> {
                [OperationKind.Block] = op => ReadBlock((IBlockOperation)op)
            };

        private readonly CodeModelBuilder _codeModel;
        private readonly IFunctionContext _destination;
        private readonly MethodDeclarationSyntax _syntax;
        private readonly SemanticModel _semanticModel;

        public OperationReader(CodeModelBuilder codeModel, IFunctionContext destination, MethodDeclarationSyntax syntax)
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
            if (StatementReaderByOperationKind.TryGetValue(operation.Kind, out var reader))
            {
                return reader(operation);
            }
            
            throw new NotSupportedException($"Operation type {operation.GetType().Name} is not supported.");
        }
        
        private static AbstractExpression ReadExpression(IOperation operation)
        {
            if (ExpressionReaderByOperationKind.TryGetValue(operation.Kind, out var reader))
            {
                return reader(operation);
            }
            
            throw new NotSupportedException($"Operation type {operation.GetType().Name} is not supported.");
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
            var context = CodeGeneratorContext.GetContextOrThrow();
            var result = new BlockStatement();

            using (context.PushState(new BlockContext(result)))
            {
                foreach (var childOp in op.Operations)
                {
                    result.Statements.Add(ReadStatement(childOp));
                }
            }

            return result;
        }

        private static AbstractExpression ReadAssignment(IAssignmentOperation op)
        {
            var context = CodeGeneratorContext.GetContextOrThrow();
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
    }
}