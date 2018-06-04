using System.Collections.Generic;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Fluent
{
    public class NamespaceContext
    {
        public NamespaceContext(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}