using System.Collections.Generic;
using System.Collections.Immutable;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Statements
{
    public class BlockStatement : AbstractStatement
    {
        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitBlockStatement(this);

            foreach (var statement in this.Statements)
            {
                statement.AcceptVisitor(visitor);
            }
        }

        public List<LocalVariable> Locals { get; } = new List<LocalVariable>();
        public List<AbstractStatement> Statements { get; } = new List<AbstractStatement>();
    }
}
