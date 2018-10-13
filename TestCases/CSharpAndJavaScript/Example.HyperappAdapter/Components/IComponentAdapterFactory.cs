using Example.WebUIModel.Metadata;

namespace Example.HyperappAdapter.Components
{
    public interface IComponentAdapterFactory
    {
        IComponentAdapter CreateComponentAdapter(WebComponentMetadata metadata);
    }
}