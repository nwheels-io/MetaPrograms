using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative
{
    public class ImperativeCodeModel
    {
        private readonly List<IMemberRef> _topLevelMembers;
        private readonly Dictionary<object, IMemberRef> _membersByBndings;

        public ImperativeCodeModel(
            IEnumerable<AbstractMember> topLevelMembers, 
            IDictionary<object, MemberRef<AbstractMember>> membersByBindings = null)
        {
            _topLevelMembers = topLevelMembers.Select(m => (IMemberRef)m.GetAbstractRef()).ToList();
            _membersByBndings = (
                membersByBindings?.ToDictionary(kvp => kvp.Key, kvp => (IMemberRef)kvp.Value) ??
                new Dictionary<object, IMemberRef>());
        }

        public IReadOnlyList<IMemberRef> TopLevelMembers => _topLevelMembers;
        public IReadOnlyDictionary<object, IMemberRef> MembersByBndings => _membersByBndings;

        public MemberRef<TMember> Get<TMember>(object binding)
            where TMember : AbstractMember
        {
            return MembersByBndings[binding].AsRef<TMember>();
        }

        public void Add<TMember>(MemberRef<TMember> member, bool isTopLevel = false)
            where TMember : AbstractMember
        {
            if (isTopLevel)
            {
                _topLevelMembers.Add(member);
            }

            foreach (var binding in member.Get().Bindings)
            {
                _membersByBndings.Add(binding, member);
            }
        }
    }
}
