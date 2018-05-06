using System;
using System.Linq;
using System.Reflection;
using MetaPrograms.CodeModel.Imperative.Members;

namespace Example.WebUIModel.Metadata
{
    public class WebComponentMetadata
    {
        public WebComponentMetadata(TypeMember componentClass)
        {
            this.ComponentClass = componentClass;
            IsPredefined = (
                componentClass.Bindings.OfType<Type>().Single().GetCustomAttribute<ProgrammingModelAttribute>() != null);
        }

        public TypeMember ComponentClass { get; }
        public bool IsPredefined { get; }
    }
}