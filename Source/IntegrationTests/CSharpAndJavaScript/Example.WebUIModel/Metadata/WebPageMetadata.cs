using System;
using System.Collections.Immutable;
using System.Linq;
using MetaPrograms.CodeModel.Imperative;
using MetaPrograms.CodeModel.Imperative.Expressions;
using MetaPrograms.CodeModel.Imperative.Members;
using MetaPrograms.CodeModel.Imperative.Statements;

namespace Example.WebUIModel.Metadata
{
    public class WebPageMetadata
    {
        private readonly ImperativeCodeModel _imperativeCodeModel;
        private readonly IBackendApiRegistry _apiRegistry;

        public WebPageMetadata(ImperativeCodeModel imperativeCodeModel, IBackendApiRegistry apiRegistry, TypeMember pageClass)
        {
            _imperativeCodeModel = imperativeCodeModel;
            _apiRegistry = apiRegistry;

            this.PageClass = pageClass;
            this.StateClass = pageClass.BaseType.GenericArguments[0];
            this.ControllerMethod = TryFindControllerMethod();
            this.ModelProperty = FindModelProperty();
            this.Components = DiscoverComponents();
            this.BackendApis = DiscoverBackendApis();

            this.IsIndex = pageClass.HasAttribute<WebUI.Semantic.IndexPageAttribute>();

            if (ModelProperty != null)
            {
                this.ModelMetadata = new ModelObjectMetadata(ModelProperty.PropertyType);
            }
        }

        public TypeMember PageClass { get; }
        public TypeMember StateClass { get; }
        public bool IsIndex { get; }
        public ImmutableArray<WebComponentMetadata> Components { get; }
        public ImmutableArray<WebApiMetadata> BackendApis { get; }
        public PropertyMember ModelProperty { get; }
        public MethodMember ControllerMethod { get; }
        public ModelObjectMetadata ModelMetadata { get; }
        public MetadataExtensionMap Extensions { get; } = new MetadataExtensionMap();

        private ImmutableArray<WebComponentMetadata> DiscoverComponents()
        {
            //TODO: add TypeMember.Properties : IEnumerable<PropertyMember>
            return PageClass.Members
                .OfType<PropertyMember>()
                .Where(IsComponentProperty)
                .Select(property => new WebComponentMetadata(this, _imperativeCodeModel, property, this.ControllerMethod))
                .ToImmutableArray();

            bool IsComponentProperty(PropertyMember property)
            {
                //TODO: add PropertyMember.IsGetOnly
                return (
                    (property.Modifier & MemberModifier.Static) == 0 && //TODO: add PropertyMember.IsStatic / IsInstance
                    property.Getter != null &&
                    IsComponentClass(property.PropertyType));
            }

            bool IsComponentClass(TypeMember type)
            {
                if (type.TypeKind != TypeMemberKind.Class)
                {
                    return false;
                }

                //TODO: add TypeMember.IsA()
                for (var baseType = type.BaseType; baseType != null; baseType = baseType.BaseType)
                {
                    if (baseType.Bindings.FirstOrDefault<Type>() == typeof(AbstractComponent))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        private ImmutableArray<WebApiMetadata> DiscoverBackendApis()
        {
            //TODO: add TypeMember.Properties : IEnumerable<PropertyMember>
            return PageClass.Members
                .OfType<PropertyMember>()
                .Select(TryGetApiType)
                .Where(t => t != null)
                .Select(t => _apiRegistry.GetApiMetadata(t))
                .ToImmutableArray();

            TypeMember TryGetApiType(PropertyMember property)
            {
                if (property.Getter != null &&
                    property.Getter.Body.Statements.Count == 1 &&
                    property.Getter.Body.Statements[0] is ReturnStatement returnStatement &&
                    returnStatement.Expression is MethodCallExpression methodCall &&
                    methodCall.Target is ThisExpression &&
                    methodCall.Method?.Name == "GetBackendApiProxy")
                {
                    return property.PropertyType;
                }

                return null;
            }
        }

        private MethodMember TryFindControllerMethod()
        {
            //TODO: add TypeMember.Methods : IEnumerable<MethodMember> + Fields, Properties, Events, NestedTypes
            return PageClass.Members
                .OfType<MethodMember>()
                .FirstOrDefault(IsControllerMethod);

            bool IsControllerMethod(MethodMember method)
            {
                //TODO: add MethodMember.IsOverrideOf(MethodMember / MethodInfo)
                return (
                    method.Signature.IsVoid &&
                    method.Signature.Parameters.Count == 0 &&
                    method.Modifier == MemberModifier.Override &&
                    method.Name == "Controller");
            }
        }

        private PropertyMember FindModelProperty()
        {
            return PageClass
                .BaseType.BaseType
                .Members.OfType<PropertyMember>()
                .FirstOrDefault(IsModelProperty);

            bool IsModelProperty(PropertyMember property)
            {
                return (property.Name == "Model");
                    // && property.DeclaringType.Bindings.OfType<Type>().FirstOrDefault() == typeof(WebPage<>))
            }
        }
    }
}
