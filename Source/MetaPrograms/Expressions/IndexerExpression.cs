using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using MetaPrograms.Members;

namespace MetaPrograms.Expressions
{
    public class IndexerExpression : AbstractExpression
    {
        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitIndexerExpression(this);

            if (Target != null)
            {
                Target.AcceptVisitor(visitor);
            }

            foreach (var argument in IndexArguments)
            {
                argument.AcceptVisitor(visitor);
            }
        }

        public override AbstractExpression AcceptRewriter(StatementRewriter rewriter)
        {
            return rewriter.RewriteIndexerExpression(this);
        }

        public AbstractExpression Target { get; set; }
        public List<AbstractExpression> IndexArguments { get; } = new List<AbstractExpression>();

        public AbstractExpression Index
        {
            get
            {
                if (IndexArguments.Count == 0)
                {
                    throw new InvalidOperationException("Index arguments were not set");
                }

                if (IndexArguments.Count != 1)
                {
                    throw new InvalidOperationException("This is a multiple-argument indexer");
                }

                return IndexArguments[0];
            }
        }
    }
}
