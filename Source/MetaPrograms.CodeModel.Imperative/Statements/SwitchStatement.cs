using System.Collections.Generic;
using System.Collections.Immutable;

namespace MetaPrograms.CodeModel.Imperative.Statements
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

        public List<SwitchCaseBlock> CaseBlocks { get; } = new List<SwitchCaseBlock>();
    }
}
