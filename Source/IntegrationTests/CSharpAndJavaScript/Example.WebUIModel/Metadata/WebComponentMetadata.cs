using System;
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
            
            //TODO: add AbstractMember.HasAttribute<T>()/TryGetAttribute<T>()
            this.IsPredefined = (
                ComponentClass.Bindings.FirstOrDefault<Type>()?.GetCustomAttribute<ProgrammingModelAttribute>() != null);
        }

        public PropertyMember DeclaredProperty { get; }
        public TypeMember ComponentClass { get; }
        public bool IsPredefined { get; }
    }
}