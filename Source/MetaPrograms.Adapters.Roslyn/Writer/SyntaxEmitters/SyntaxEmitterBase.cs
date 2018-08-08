using Microsoft.CodeAnalysis;

namespace MetaPrograms.Adapters.Roslyn.Writer.SyntaxEmitters
{
    public abstract class SyntaxEmitterBase<TSyntax> : ISyntaxEmitter<TSyntax>, ISyntaxEmitter
        where TSyntax : SyntaxNode
    {
        public abstract TSyntax EmitSyntax();

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        SyntaxNode ISyntaxEmitter.EmitSyntax()
        {
            return this.EmitSyntax();
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public TSyntax OutputSyntax { get; protected set; }
    }
}
