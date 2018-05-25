using System;
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
        public TMember Get<TMember>(object binding) => throw new NotImplementedException();
    }
}
