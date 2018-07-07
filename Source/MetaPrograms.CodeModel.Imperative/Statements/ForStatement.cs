using System.Collections.Generic;
using System.Collections.Immutable;
using MetaPrograms.CodeModel.Imperative.Expressions;

namespace MetaPrograms.CodeModel.Imperative.Statements
{
    public class ForStatement : AbstractStatement
    {
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

        public List<AbstractStatement> Initializers { get; } = new List<AbstractStatement>();
        public AbstractExpression Condition { get; set; }
        public List<AbstractStatement> Iterators { get; } = new List<AbstractStatement>();
        public BlockStatement Body { get; set; }
    }
}
