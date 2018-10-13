using System.Collections.Generic;
using MetaPrograms.Members;

namespace MetaPrograms.Fluent
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