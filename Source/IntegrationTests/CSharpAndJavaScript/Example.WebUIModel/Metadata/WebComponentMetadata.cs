using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using MetaPrograms.CodeModel.Imperative;
using MetaPrograms.CodeModel.Imperative.Members;

namespace Example.WebUIModel.Metadata
{
    public class WebComponentMetadata
    {
        private readonly ImperativeCodeModel _imperativeCodeModel;

        public WebComponentMetadata(ImperativeCodeModel imperativeCodeModel, PropertyMember declaredProperty)
        {
            _imperativeCodeModel = imperativeCodeModel;

            this.DeclaredProperty = declaredProperty;
            this.ComponentClass = declaredProperty.PropertyType;
            this.ModelClass = ComponentClass.GenericArguments.FirstOrDefault();

            //TODO: add AbstractMember.HasAttribute<T>()/TryGetAttribute<T>()
            this.IsPredefined = (
                ComponentClass.Bindings.FirstOrDefault<Type>()?.GetCustomAttribute<ProgrammingModelAttribute>() != null);
        }

        public PropertyMember DeclaredProperty { get; }
        public TypeMember ComponentClass { get; }
        public TypeMember ModelClass { get; }
        public bool IsPredefined { get; }
    }
}