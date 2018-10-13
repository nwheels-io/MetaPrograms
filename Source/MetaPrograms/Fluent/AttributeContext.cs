using System.Collections.Generic;
using MetaPrograms.Members;

namespace MetaPrograms.Fluent
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
