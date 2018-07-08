using System.Collections.Immutable;
using System.Linq;
using MetaPrograms.CodeModel.Imperative;
using MetaPrograms.CodeModel.Imperative.Members;

namespace Example.WebUIModel.Metadata
{
    public class WebApiMetadata
    {
        private readonly ImperativeCodeModel _imperativeCodeModel;

        public WebApiMetadata(ImperativeCodeModel imperativeCodeModel, TypeMember interfaceType)
        {
            _imperativeCodeModel = imperativeCodeModel;
            
            this.InterfaceType = interfaceType;
            this.ApiMethods = DiscoverApiMethods();
        }

        public TypeMember InterfaceType { get; }
        public ImmutableList<MethodMember> ApiMethods { get; }

        private ImmutableList<MethodMember> DiscoverApiMethods()
        {
            return InterfaceType.Members
                .OfType<MethodMember>()
                .ToImmutableList();
        }
    }
}
