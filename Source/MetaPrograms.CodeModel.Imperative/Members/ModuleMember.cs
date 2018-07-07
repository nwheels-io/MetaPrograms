using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace MetaPrograms.CodeModel.Imperative.Members
{
    public class ModuleMember : TypeMember
    {
        public List<ImportDirective> Imports { get; } = new List<ImportDirective>();
    }
}
