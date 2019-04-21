using System.Collections.Immutable;
using System.Linq;
using MetaPrograms;
using MetaPrograms.Members;

namespace Example.WebUIModel.Metadata
{
    public class WebApiMetadata
    {
        private readonly ImperativeCodeModel _imperativeCodeModel;

        public WebApiMetadata(ImperativeCodeModel imperativeCodeModel, TypeMember interfaceType)
        {
            _imperativeCodeModel = imperativeCodeModel;

            this.InterfaceType = interfaceType;
            this.ServiceName = interfaceType.Name.TrimPrefixFragment("I").TrimSuffixFragment("Service");
            this.ApiMethods = DiscoverApiMethods();
        }

        public string GetMethodUrl(MethodMember method) => 
            $"/api/{ServiceName.AppendSuffixFragments("Service").ToString(CasingStyle.Camel)}/{method.Name.ToString(CasingStyle.Camel)}";

        public TypeMember InterfaceType { get; }
        public ImmutableList<MethodMember> ApiMethods { get; }
        public IdentifierName ServiceName { get; }

        private ImmutableList<MethodMember> DiscoverApiMethods()
        {
            return InterfaceType.Members
                .OfType<MethodMember>()
                .ToImmutableList();
        }
    }
}
