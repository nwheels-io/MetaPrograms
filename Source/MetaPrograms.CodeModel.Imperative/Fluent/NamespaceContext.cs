using System.Collections.Generic;

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