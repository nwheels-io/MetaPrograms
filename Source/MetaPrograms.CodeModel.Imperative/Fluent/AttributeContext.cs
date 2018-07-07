using System.Collections.Generic;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Fluent
{
    public class AttributeContext
    {
        public AttributeContext(AttributeDescription attribute)
        {
            Attribute = attribute;
        }

        public AttributeDescription Attribute { get; }
    }
}
