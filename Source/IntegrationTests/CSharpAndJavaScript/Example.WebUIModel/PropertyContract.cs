using System;
using System.Collections.Generic;
using System.Text;

namespace Example.WebUIModel
{
    public static class PropertyContract
    {
        public static class Validation
        {
            public class RequiredAttribute : Attribute
            {
            }
        }

        public static class Semantic
        {
            public class InputAttribute : Attribute
            {
            }
            public class OutputAttribute : Attribute
            {
            }
        }

        public static class Presentation
        {
            public class HideIfNullOrEmptyAttribute : Attribute
            {
            }
        }

        public static class I18n
        {
            public class LabelAttribute : Attribute
            {
                public LabelAttribute(string pascalCaseId)
                {
                }
            }
        }
    }
}
