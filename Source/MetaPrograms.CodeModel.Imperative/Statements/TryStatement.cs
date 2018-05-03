using System.Collections.Generic;
using System.Collections.Immutable;

namespace MetaPrograms.CodeModel.Imperative.Statements
{
    public class TryStatement : AbstractStatement
    {
        public TryStatement(
            BlockStatement tryBlock,
            ImmutableList<TryCatchBlock> catchBlocks, 
            BlockStatement finallyBlock)
        {
            TryBlock = tryBlock;
            CatchBlocks = catchBlocks;
            FinallyBlock = finallyBlock;
        }

        public TryStatement(
            TryStatement source,
            Mutator<BlockStatement>? tryBlock = null,
            Mutator<ImmutableList<TryCatchBlock>>? catchBlocks = null,
            Mutator<BlockStatement>? finallyBlock = null)
        {
            TryBlock = tryBlock.MutatedOrOriginal(source.TryBlock);
            CatchBlocks = catchBlocks.MutatedOrOriginal(source.CatchBlocks);
            FinallyBlock = finallyBlock.MutatedOrOriginal(source.FinallyBlock);
        }

        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitTryStatement(this);

            TryBlock.AcceptVisitor(visitor);

            foreach (var catchBlock in CatchBlocks)
            {
                catchBlock.AcceptVisitor(visitor);
            }

            if (FinallyBlock != null)
            {
                FinallyBlock.AcceptVisitor(visitor);
            }
        }

        public BlockStatement TryBlock { get; }
        public ImmutableList<TryCatchBlock> CatchBlocks { get; }
        public BlockStatement FinallyBlock { get; }
    }
}
