using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace MetaPrograms.CodeModel.Imperative.Members
{
    public class ModuleMember : AbstractMember
    {
        public string[] FolderPath { get; set; }
        public List<ImportDirective> Imports { get; set; } = new List<ImportDirective>();
        public List<AbstractMember> Members { get; set; } = new List<AbstractMember>();
        public override bool IsTopLevel => true;
    }
}
