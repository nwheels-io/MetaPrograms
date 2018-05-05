using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative
{
    public class CodeMemberCollection : IEnumerable<AbstractMember>
    {
        private readonly ImmutableList<AbstractMember> _topLevelMembers;

        public CodeMemberCollection(IEnumerable<AbstractMember> topLevelMembers)
        {
            _topLevelMembers = topLevelMembers.ToImmutableList();
        }

        public int Count => _topLevelMembers.Count;
        
        public IEnumerator<AbstractMember> GetEnumerator()
        {
            return _topLevelMembers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}