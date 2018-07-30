using System.Xml.Linq;
using Example.WebUIModel.Metadata;

namespace Example.HyperappAdapter.Components
{
    public interface IComponentAdapter
    {
        void GenerateStateKeys();
        void GenerateActionKeys();
        XElement GenerateViewMarkup();
        WebComponentMetadata Metadata { get; }
    }
}
