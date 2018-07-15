using System.Xml.Linq;
using Example.WebUIModel;
using Example.WebUIModel.Metadata;

namespace Example.HyperappAdapter.Components
{
    public abstract class ComponentAdapter<TComponent> : IComponentAdapter
        where TComponent : AbstractComponent
    {
        public WebComponentMetadata Metadata { get; }
        public TComponent Component { get; }

        protected ComponentAdapter(WebComponentMetadata metadata, TComponent component)
        {
            this.Metadata = metadata;
            this.Component = component;
        }

        public abstract void GenerateStateKey();
        public abstract void GenerateActionsKey();
        public abstract XElement GenerateViewMarkup();
    }
}