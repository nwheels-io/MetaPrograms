using System.Collections.Generic;
using System.Collections.Immutable;
using MetaPrograms.CodeModel.Imperative.Members;
using MetaPrograms.CodeModel.Imperative.Statements;

namespace MetaPrograms.CodeModel.Imperative.Fluent
{
    public class BlockContainer : IBlockContainerContext
    {
        void IBlockContainerContext.AttachBlock(IEnumerable<LocalVariable> locals, IEnumerable<AbstractStatement> statements)
        {
            Block = new BlockStatement(locals.ToImmutableList(), statements.ToImmutableList());
        }

        public BlockStatement Block { get; private set; }
    }
}