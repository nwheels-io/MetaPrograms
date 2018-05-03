using System.Collections.Generic;
using System.Collections.Immutable;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Statements
{
    public class BlockStatement : AbstractStatement
    {
        public BlockStatement(
            ImmutableList<LocalVariable> locals, 
            ImmutableList<AbstractStatement> statements)
        {
            this.Locals = locals;
            this.Statements = statements;
        }

        public BlockStatement(
            BlockStatement source,
            Mutator<ImmutableList<LocalVariable>>? locals = null,
            Mutator<ImmutableList<AbstractStatement>>? statements = null)
        {
            this.Locals = locals.MutatedOrOriginal(source.Locals);
            this.Statements = statements.MutatedOrOriginal(source.Statements);
        }

        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitBlockStatement(this);

            foreach (var statement in this.Statements)
            {
                statement.AcceptVisitor(visitor);
            }
        }

        public ImmutableList<LocalVariable> Locals { get; }
        public ImmutableList<AbstractStatement> Statements { get; }
    }
}
