using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace MetaPrograms.Adapters.Roslyn.Reader
{
    public class TypeDiscoveryOperationWalker : OperationWalker
    {
        private readonly Action<ISymbol> _symbolCallback;

        public TypeDiscoveryOperationWalker(Action<ISymbol> symbolCallback)
        {
            _symbolCallback = symbolCallback;
        }

        public override void VisitVariableDeclarator(IVariableDeclaratorOperation operation)
        {
            _symbolCallback(operation.Symbol.Type);
            base.VisitVariableDeclarator(operation);
        }

        public override void VisitLocalReference(ILocalReferenceOperation operation)
        {
            _symbolCallback(operation.Local.Type);
            base.VisitLocalReference(operation);
        }

        public override void VisitIsPattern(IIsPatternOperation operation)
        {
            base.VisitIsPattern(operation);
        }

        public override void VisitDeclarationPattern(IDeclarationPatternOperation operation)
        {
            _symbolCallback(operation.DeclaredSymbol);
            base.VisitDeclarationPattern(operation);
        }

        public override void VisitPatternCaseClause(IPatternCaseClauseOperation operation)
        {
            base.VisitPatternCaseClause(operation);
        }
    }
}
