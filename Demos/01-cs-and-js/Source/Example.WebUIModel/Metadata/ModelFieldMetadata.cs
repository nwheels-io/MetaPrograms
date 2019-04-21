using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MetaPrograms;
using MetaPrograms.Members;
using static Example.WebUIModel.PropertyContract;

namespace Example.WebUIModel.Metadata
{
    public class ModelFieldMetadata
    {
        public ModelObjectMetadata Object { get; }
        public PropertyMember Property { get; }
        public FieldDirection Direction { get; }
        public string Label { get; }
        public bool IsRequired { get; }
        public bool AllowEmpty { get; }
        public int? MaxLength { get; }

        public ModelFieldMetadata(ModelObjectMetadata @object, PropertyMember field)
        {
            var outputAttribute = field.TryGetAttribute<Semantic.OutputAttribute>();
            var labelAttribute = field.TryGetAttribute<I18n.LabelAttribute>();
            var requiredAttribute = field.TryGetAttribute<Validation.RequiredAttribute>();

            this.Object = @object;
            this.Property = field;
            this.Direction = (outputAttribute != null ? FieldDirection.Output : FieldDirection.Input);
            this.Label = (labelAttribute != null
                ? labelAttribute.GetConstructorArgumentOrDefault<string>(defaultValue: field.Name)
                : field.Name.ToString(CasingStyle.Pascal));

            if (requiredAttribute != null)
            {
                this.IsRequired = true;
                this.AllowEmpty = requiredAttribute.GetPropertyValueOrDefault<bool>(Validation.RequiredAttribute.AllowEmptyPropertyName);
                
                var tempMaxLength = requiredAttribute.GetPropertyValueOrDefault<int>(
                    Validation.RequiredAttribute.MaxLengthPropertyName, 
                    defaultValue: -1);

                this.MaxLength = (tempMaxLength >= 0 ? (int?)tempMaxLength : null);
            }
            else
            {
                this.AllowEmpty = true;
            }
        }

        public override string ToString()
        {
            return Property.Name;
        }
    }

    public enum FieldDirection
    {
        Input,
        Output
    }
}
