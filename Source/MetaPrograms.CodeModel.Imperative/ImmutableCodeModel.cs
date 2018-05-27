using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative
{
    public class ImmutableCodeModel
    {
        public ImmutableCodeModel(
            IEnumerable<AbstractMember> topLevelMembers, 
            IDictionary<object, MemberRef<AbstractMember>> membersByBindings = null)
        {
            TopLevelMembers = topLevelMembers.ToImmutableList();
            MembersByBndings =
                membersByBindings?.ToImmutableDictionary(kvp => kvp.Key, kvp => kvp.Value.Get()) ??
                ImmutableDictionary<object, AbstractMember>.Empty;
        }

        public ImmutableList<AbstractMember> TopLevelMembers { get; }
        public ImmutableDictionary<object, AbstractMember> MembersByBndings { get; }
        
        public TMember Get<TMember>(object binding) 
            where TMember : AbstractMember
            => (TMember)MembersByBndings[binding];
    }
}
