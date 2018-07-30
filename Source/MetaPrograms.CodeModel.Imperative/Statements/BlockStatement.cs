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

        public override AbstractStatement AcceptRewriter(StatementRewriter rewriter)
        {
            return rewriter.RewriteBlockStatement(this);
        }

        public List<LocalVariable> Locals { get; set; } = new List<LocalVariable>();
        public List<AbstractStatement> Statements { get; set; } = new List<AbstractStatement>();
    }
}
