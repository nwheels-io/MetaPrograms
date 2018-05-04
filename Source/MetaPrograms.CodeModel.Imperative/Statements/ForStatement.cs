using System.Collections.Generic;
using System.Collections.Immutable;
using MetaPrograms.CodeModel.Imperative.Expressions;

namespace MetaPrograms.CodeModel.Imperative.Statements
{
    public class ForStatement : AbstractStatement
    {
        public ForStatement(
            ImmutableList<AbstractStatement> initializers, 
            AbstractExpression condition, 
            ImmutableList<AbstractStatement> iterators, 
            BlockStatement body)
        {
            Initializers = initializers;
            Condition = condition;
            Iterators = iterators;
            Body = body;
        }

        public ForStatement(
            ForStatement source,
            Mutator<ImmutableList<AbstractStatement>>? initializers = null,
            Mutator<AbstractExpression>? condition = null,
            Mutator<ImmutableList<AbstractStatement>>? iterators = null,
            Mutator<BlockStatement>? body = null)
        {
            Initializers = initializers.MutatedOrOriginal(source.Initializers);
            Condition = condition.MutatedOrOriginal(source.Condition);
            Iterators = iterators.MutatedOrOriginal(source.Iterators);
            Body = body.MutatedOrOriginal(source.Body);
        }

        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitForStatement(this);

            foreach (var initializer in Initializers)
            {
                initializer.AcceptVisitor(visitor);
            }

            if (Condition != null)
            {
                Condition.AcceptVisitor(visitor);
            }

            foreach (var iterator in Iterators)
            {
                iterator.AcceptVisitor(visitor);
            }

            Body.AcceptVisitor(visitor);
        }

        public ImmutableList<AbstractStatement> Initializers { get; }
        public AbstractExpression Condition { get; }
        public ImmutableList<AbstractStatement> Iterators { get; }
        public BlockStatement Body { get; }
    }
}
