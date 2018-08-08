using Microsoft.CodeAnalysis;

namespace MetaPrograms.Adapters.Roslyn.Writer.SyntaxEmitters
{
    public interface ISyntaxEmitter
    {
        SyntaxNode EmitSyntax(); 
    }

    public interface ISyntaxEmitter<TSyntax>
        where TSyntax : SyntaxNode
    {
        TSyntax EmitSyntax();
    }
}
