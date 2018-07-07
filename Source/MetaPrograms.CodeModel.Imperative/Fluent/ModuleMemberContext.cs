using System;
using System.Collections.Generic;
using System.Text;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Fluent
{
    public class ModuleMemberContext
    {
        private readonly List<AbstractMember> _members = new List<AbstractMember>();

        public ModuleMemberContext(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public void AddMember(AbstractMember member)
        {
            _members.Add(member);
        }

        public IEnumerable<AbstractMember> Members => _members;
    }
}
