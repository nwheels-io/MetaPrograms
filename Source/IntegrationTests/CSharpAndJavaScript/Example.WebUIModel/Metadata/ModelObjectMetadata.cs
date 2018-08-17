using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using MetaPrograms.CodeModel.Imperative.Members;

namespace Example.WebUIModel.Metadata
{
    public class ModelObjectMetadata
    {
        public ModelObjectMetadata(TypeMember type)
        {
            this.Type = type;
            this.Fields = type.Members
                .OfType<PropertyMember>()
                .Select(field => new ModelFieldMetadata(this, field))
                .ToImmutableList();
        }

        public TypeMember Type { get; }
        public ImmutableList<ModelFieldMetadata> Fields { get; }

        public override string ToString()
        {
            return Type.FullName;
        }
    }
}
