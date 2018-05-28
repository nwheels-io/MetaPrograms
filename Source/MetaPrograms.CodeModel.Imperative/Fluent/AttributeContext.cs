using System.Collections.Generic;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Fluent
{
    public class AttributeContext
    {
        public AttributeContext()
        {
            NamedProperties = new List<NamedPropertyValue>();
        }

        public List<NamedPropertyValue> NamedProperties { get; }
    }
}
