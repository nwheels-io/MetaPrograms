using System.Collections.Immutable;
using System.Linq;
using MetaPrograms.CodeModel.Imperative;
using MetaPrograms.CodeModel.Imperative.Members;

namespace Example.WebUIModel.Metadata
{
    public class WebApiMetadata
    {
        private readonly ImmutableCodeModel _codeModel;

        public WebApiMetadata(ImmutableCodeModel codeModel, TypeMember interfaceType)
        {
            _codeModel = codeModel;
            
            this.InterfaceType = interfaceType;
            this.ApiMethods = DiscoverApiMethods();
        }

        public TypeMember InterfaceType { get; }
        public ImmutableArray<MethodMember> ApiMethods { get; }

        private ImmutableArray<MethodMember> DiscoverApiMethods()
        {
            return InterfaceType.Members
                .Select(m => m.Get())
                .OfType<MethodMember>()
                .ToImmutableArray();
        }
    }
}
