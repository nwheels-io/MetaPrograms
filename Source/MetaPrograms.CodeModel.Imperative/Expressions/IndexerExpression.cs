using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Expressions
{
    public class IndexerExpression : AbstractExpression
    {
        public IndexerExpression(
        MemberRef<TypeMember> type, 
            AbstractExpression target,
            ImmutableList<AbstractExpression> indexArguments) 
            : base(type)
        {
            Target = target;
            IndexArguments = indexArguments;
        }

        public IndexerExpression(
            IndexerExpression source, 
            Mutator<MemberRef<TypeMember>>? type = null,
            Mutator<AbstractExpression>? target = null,
            Mutator<ImmutableList<AbstractExpression>>? indexArguments = null) 
            : base(source, type)
        {
            Target = target.MutatedOrOriginal(source.Target);
            IndexArguments = indexArguments.MutatedOrOriginal(source.IndexArguments);
        }

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

        public AbstractExpression Target { get; }
        public ImmutableList<AbstractExpression> IndexArguments { get; }

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
