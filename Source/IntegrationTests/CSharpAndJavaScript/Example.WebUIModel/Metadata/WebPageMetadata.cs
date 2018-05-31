using System;
using System.Collections.Immutable;
using System.Linq;
using MetaPrograms.CodeModel.Imperative;
using MetaPrograms.CodeModel.Imperative.Members;

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
            this.StateClass = pageClass.BaseType.Get().GenericArguments[0];

            this.Components = DiscoverComponents();
            this.BackendApis = DiscoverBackendApis();
            this.ControllerMethod = TryFindControllerMethod();
        }

        public TypeMember PageClass { get; }
        public TypeMember StateClass { get; }
        public ImmutableArray<WebComponentMetadata> Components { get; } 
        public ImmutableArray<WebApiMetadata> BackendApis { get; } 
        public MethodMember ControllerMethod { get; } 

        private ImmutableArray<WebComponentMetadata> DiscoverComponents()
        {
            //TODO: add TypeMember.Properties : IEnumerable<PropertyMember>
            return PageClass.Members
                .Select(m => m.Get())
                .OfType<PropertyMember>()
                .Where(IsComponentProperty)
                .Select(property => new WebComponentMetadata(_imperativeCodeModel, property))
                .ToImmutableArray();

            bool IsComponentProperty(PropertyMember property)
            {
                //TODO: add PropertyMember.IsGetOnly
                return (
                    (property.Modifier & MemberModifier.Static) == 0 && //TODO: add PropertyMember.IsStatic / IsInstance
                    property.Getter.IsNotNull &&
                    property.Setter.IsNull &&
                    IsComponentClass(property.PropertyType));
            }
            
            bool IsComponentClass(TypeMember type)
            {
                if (type.TypeKind != TypeMemberKind.Class)
                {
                    return false;
                }
                
                //TODO: add TypeMember.IsA()
                for (var baseType = type.BaseType.Get(); baseType != null; baseType = baseType.BaseType.Get())
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
                .Select(m => m.Get())
                .OfType<PropertyMember>()
                .Select(TryGetApiType)
                .Where(t => t != null)
                .Select(t => _apiRegistry.GetApiMetadata(t))
                .ToImmutableArray();
            
            TypeMember TryGetApiType(PropertyMember property)
            {
                var type = property.PropertyType.Get();

                //TODO: add TypeMember.IsA(System.Type) + support open generic System.Type
                if (type?.TypeKind == TypeMemberKind.Class && type.IsGenericType)
                {
                    var clrType = type.Bindings.FirstOrDefault<Type>();
                    if (clrType != null && clrType.GetGenericTypeDefinition() == typeof(BackendApi<>))
                    {
                        return type.GenericArguments[0];
                    }
                }
                
                return null;
            }
        }

        private MethodMember TryFindControllerMethod()
        {
            //TODO: add TypeMember.Methods : IEnumerable<MethodMember> + Fields, Properties, Events, NestedTypes
            return PageClass.Members
                .Select(m => m.Get())
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
    }
}
