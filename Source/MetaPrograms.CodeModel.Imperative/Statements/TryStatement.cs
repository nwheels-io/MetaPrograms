﻿using System.Collections.Generic;
using System.Collections.Immutable;

namespace MetaPrograms.CodeModel.Imperative.Statements
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

        public BlockStatement TryBlock { get; set; }
        public List<TryCatchBlock> CatchBlocks { get; } = new List<TryCatchBlock>();
        public BlockStatement FinallyBlock { get; set; }
    }
}
