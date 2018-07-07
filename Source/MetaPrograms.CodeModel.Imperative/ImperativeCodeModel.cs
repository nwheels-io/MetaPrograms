using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative
{
    public class ImperativeCodeModel
    {
        private readonly List<AbstractMember> _topLevelMembers;
        private readonly Dictionary<object, AbstractMember> _membersByBndings;

        public ImperativeCodeModel(
            IEnumerable<AbstractMember> topLevelMembers, 
            IDictionary<object, AbstractMember> membersByBindings = null)
        {
            _topLevelMembers = topLevelMembers.ToList();
            _membersByBndings = (
                membersByBindings?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value) ??
                new Dictionary<object, AbstractMember>());
        }

        public IReadOnlyList<AbstractMember> TopLevelMembers => _topLevelMembers;
        public IReadOnlyDictionary<object, AbstractMember> MembersByBndings => _membersByBndings;

        public TMember Get<TMember>(object binding)
            where TMember : AbstractMember
        {
            return (TMember)MembersByBndings[binding];
        }

        public void Add<TMember>(TMember member, bool isTopLevel = false)
            where TMember : AbstractMember
        {
            if (isTopLevel)
            {
                _topLevelMembers.Add(member);
            }

            foreach (var binding in member.Bindings)
            {
                _membersByBndings.Add(binding, member);
            }
        }
    }
}
