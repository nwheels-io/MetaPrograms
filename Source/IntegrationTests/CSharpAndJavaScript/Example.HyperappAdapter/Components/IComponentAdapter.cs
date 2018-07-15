using System.Xml.Linq;
using Example.WebUIModel.Metadata;

namespace Example.HyperappAdapter.Components
{
    public interface IComponentAdapter
    {
        void GenerateStateKey();
        void GenerateActionsKey();
        XElement GenerateViewMarkup();
        WebComponentMetadata Metadata { get; }
    }
}
