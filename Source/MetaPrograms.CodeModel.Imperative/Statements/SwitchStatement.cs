using System.Collections.Generic;
using System.Collections.Immutable;

namespace MetaPrograms.CodeModel.Imperative.Statements
{
    public class SwitchStatement : AbstractStatement
    {
        public SwitchStatement(ImmutableList<SwitchCaseBlock> caseBlocks)
        {
            CaseBlocks = caseBlocks;
        }

        public SwitchStatement(
            SwitchStatement source,
            Mutator<ImmutableList<SwitchCaseBlock>>? caseBlocks = null)
        {
            CaseBlocks = caseBlocks.MutatedOrOriginal(source.CaseBlocks);
        }

        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitSwitchStatement(this);

            foreach (var caseBlock in this.CaseBlocks)
            {

            }
        }

        public ImmutableList<SwitchCaseBlock> CaseBlocks { get; }
    }
}
