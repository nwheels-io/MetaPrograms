using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace MetaPrograms.Adapters.Roslyn.Reader
{
    public class TypeDiscoveryOperationWalker : OperationWalker
    {
        private static readonly ISymbol[] EmptySymbols = new ISymbol[0];

        private readonly Action<ISymbol> _symbolCallback;

        public TypeDiscoveryOperationWalker(Action<ISymbol> symbolCallback)
        {
            _symbolCallback = symbolCallback;
        }

        public override void VisitVariableDeclarator(IVariableDeclaratorOperation operation)
        {
            Callback(operation.Symbol.Type);
            base.VisitVariableDeclarator(operation);
        }

        public override void VisitLocalReference(ILocalReferenceOperation operation)
        {
            Callback(operation.Local.Type);
            base.VisitLocalReference(operation);
        }

        public override void VisitInvocation(IInvocationOperation operation)
        {
            CallbackMany(
                single: operation.TargetMethod.ContainingType,
                multiple: operation.TargetMethod.TypeArguments,
                single2: operation.TargetMethod);

            base.VisitInvocation(operation);
        }

        public override void VisitObjectCreation(IObjectCreationOperation operation)
        {
            Callback(operation.Type);
            base.VisitObjectCreation(operation);
        }

        public override void VisitArrayCreation(IArrayCreationOperation operation)
        {
            Callback(operation.Type, operation.Initializer?.Type);
            base.VisitArrayCreation(operation);
        }

        public override void VisitIsPattern(IIsPatternOperation operation)
        {
            base.VisitIsPattern(operation);
        }

        public override void VisitDeclarationPattern(IDeclarationPatternOperation operation)
        {
            Callback(operation.DeclaredSymbol);
            base.VisitDeclarationPattern(operation);
        }

        public override void VisitPatternCaseClause(IPatternCaseClauseOperation operation)
        {
            base.VisitPatternCaseClause(operation);
        }

        private void Callback(params ISymbol[] symbols)
        {
            foreach (var symbol in symbols)
            {
                if (symbol != null)
                {
                    _symbolCallback(symbol);
                }
            }
        }

        private void CallbackMany(
            IEnumerable<ISymbol> multiple = null,
            IEnumerable<ISymbol> multiple2 = null,
            IEnumerable<ISymbol> multiple3 = null,
            ISymbol single = null,
            ISymbol single2 = null,
            ISymbol single3 = null)
        {
            var allSymbols =
                new[] {single, single2, single3}
                    .Concat(multiple ?? EmptySymbols)
                    .Concat(multiple2 ?? EmptySymbols)
                    .Concat(multiple3 ?? EmptySymbols);

            foreach (var symbol in allSymbols)
            {
                if (symbol != null)
                {
                    _symbolCallback(symbol);
                }
            }
        }
    }
}
