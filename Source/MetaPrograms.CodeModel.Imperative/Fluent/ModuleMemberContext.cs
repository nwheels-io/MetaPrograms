using System;
using System.Collections.Generic;
using System.Text;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Fluent
{
    public class ModuleMemberContext
    {
        private readonly List<MemberRef<AbstractMember>> _members = new List<MemberRef<AbstractMember>>();

        public ModuleMemberContext(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public void AddMember(MemberRef<AbstractMember> member)
        {
            _members.Add(member);
        }

        public IEnumerable<MemberRef<AbstractMember>> Members => _members;
    }
}
