using MetaPrograms.CodeModel.Imperative.Members;

namespace Example.WebUIModel.Metadata
{
    public interface IBackendApiRegistry
    {
        WebApiMetadata GetApiMetadata(TypeMember interfaceType);
    }
}
