using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using MetaPrograms.Statements;

namespace MetaPrograms.Members
{
    public class ModuleMember : AbstractMember
    {
        public string[] FolderPath { get; set; }
        public List<ImportDirective> Imports { get; set; } = new List<ImportDirective>();
        public List<AbstractMember> Members { get; set; } = new List<AbstractMember>();
        public BlockStatement GloalBlock { get; set; } 
        public override bool IsTopLevel => true;
    }
}
