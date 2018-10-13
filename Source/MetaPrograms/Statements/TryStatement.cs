using System.Collections.Generic;
using System.Collections.Immutable;

namespace MetaPrograms.Statements
{
    public class TryStatement : AbstractStatement
    {
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

        public override AbstractStatement AcceptRewriter(StatementRewriter rewriter)
        {
            return rewriter.RewriteTryStatement(this);
        }

        public BlockStatement TryBlock { get; set; }
        public List<TryCatchBlock> CatchBlocks { get; } = new List<TryCatchBlock>();
        public BlockStatement FinallyBlock { get; set; }
    }
}
