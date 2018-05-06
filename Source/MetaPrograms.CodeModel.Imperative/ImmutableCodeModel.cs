using System.Collections.Generic;
using System.Collections.Immutable;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative
{
    public class ImmutableCodeModel
    {
        public ImmutableCodeModel(IEnumerable<AbstractMember> topLevelMembers)
        {
            TopLevelMembers = topLevelMembers.ToImmutableList();
        }

        public ImmutableList<AbstractMember> TopLevelMembers { get; }
    }
}