using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Expressions
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
