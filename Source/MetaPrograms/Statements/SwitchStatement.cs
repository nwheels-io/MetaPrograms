using System.Collections.Generic;
using System.Collections.Immutable;

namespace MetaPrograms.Statements
{
    public class SwitchStatement : AbstractStatement
    {
        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitSwitchStatement(this);

            foreach (var caseBlock in this.CaseBlocks)
            {

            }
        }

        public override AbstractStatement AcceptRewriter(StatementRewriter rewriter)
        {
            return rewriter.RewriteSwitchStatement(this);
        }

        public List<SwitchCaseBlock> CaseBlocks { get; } = new List<SwitchCaseBlock>();
    }
}
