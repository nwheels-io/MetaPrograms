using System;
using System.Reflection;
using Example.WebUIModel;
using Example.WebUIModel.Metadata;

namespace Example.HyperappAdapter.Components
{
    public class ComponentAdapterFactory : IComponentAdapterFactory
    {
        public IComponentAdapter CreateComponentAdapter(WebComponentMetadata metadata)
        {
            var componentClrType = metadata.DeclaredProperty.PropertyType.Bindings.First<Type>();

            if (componentClrType.IsGenericType && componentClrType.GetGenericTypeDefinition() == typeof(FormComponent<>))
            {
                var modelClrType = componentClrType.GenericTypeArguments[0];
                var componentAdapterType = typeof(FormComponentAdapter<>).MakeGenericType(modelClrType);
                return (IComponentAdapter)Activator.CreateInstance(componentAdapterType, new object[] { metadata, null });
            }
            
            throw new NotSupportedException($"Component type not supported: '${componentClrType.Name}'.");
        }
    }
}