using System.Collections.Generic;
using System.Collections.Immutable;
using MetaPrograms.CodeModel.Imperative.Members;
using MetaPrograms.CodeModel.Imperative.Statements;

namespace MetaPrograms.CodeModel.Imperative.Fluent
{
    public class BlockContext : BlockContextBase
    {
        protected override void AttachBlock(IEnumerable<LocalVariable> locals, IEnumerable<AbstractStatement> statements)
        {
            Block = new BlockStatement(locals.ToImmutableList(), statements.ToImmutableList());
        }

        public BlockStatement Block { get; private set; }

        public static implicit operator BlockStatement(BlockContext context)
        {
            return context.Block;
        }
    }
}