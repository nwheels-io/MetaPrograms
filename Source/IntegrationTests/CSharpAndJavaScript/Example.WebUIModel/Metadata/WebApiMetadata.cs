using System.Collections.Immutable;
using System.Linq;
using CommonExtensions;
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
            this.ServiceName = interfaceType.Name.TrimPrefixFragment("I").TrimSuffixFragment("Service").ToString();
            this.ApiMethods = DiscoverApiMethods();
        }

        public string GetMethodUrl(MethodMember method) => $"api/{ServiceName.ToLower()}/{method.Name.ToString(CasingStyle.Kebab)}";

        public TypeMember InterfaceType { get; }
        public ImmutableList<MethodMember> ApiMethods { get; }
        public string ServiceName { get; }

        private ImmutableList<MethodMember> DiscoverApiMethods()
        {
            return InterfaceType.Members
                .OfType<MethodMember>()
                .ToImmutableList();
        }
    }
}
