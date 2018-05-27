﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using MetaPrograms.CodeModel.Imperative;
using MetaPrograms.CodeModel.Imperative.Members;

namespace Example.WebUIModel.Metadata
{
    public class WebUIMetadata : IBackendApiRegistry
    {
        private readonly ImmutableCodeModel _codeModel;
        private readonly Dictionary<TypeMember, WebApiMetadata> _backendApiByType = new Dictionary<TypeMember, WebApiMetadata>();
        
        public WebUIMetadata(ImmutableCodeModel codeModel)
        {
            _codeModel = codeModel;
            
            this.Pages = DiscoverWebPages();
            this.BackendApis = _backendApiByType.Values.ToImmutableArray();
        }

        public ImmutableArray<WebPageMetadata> Pages { get; }
        public ImmutableArray<WebApiMetadata> BackendApis { get; }      

        WebApiMetadata IBackendApiRegistry.GetApiMetadata(TypeMember interfaceType)
        {
            if (_backendApiByType.TryGetValue(interfaceType, out WebApiMetadata existingApi))
            {
                return existingApi;
            }
            
            var newApi = new WebApiMetadata(_codeModel, interfaceType);
            _backendApiByType.Add(interfaceType, newApi);
            return newApi;
        }

        private ImmutableArray<WebPageMetadata> DiscoverWebPages()
        {
            var webPageClasses = _codeModel.TopLevelMembers
                .OfType<TypeMember>()
                .Where(t => t.TypeKind == TypeMemberKind.Class)
                .Where(IsWebPageClass)
                .ToArray();

            foreach (var webPageClass in webPageClasses)
            {
                Console.WriteLine($"WEB PAGE CLASS >> {webPageClass?.FullName}");
            }

            return webPageClasses
                .Select(pageClass => new WebPageMetadata(_codeModel, this, pageClass))
                .ToImmutableArray();
        }

        private bool IsWebPageClass(TypeMember type)
        {
            var baseTypeMember = type.BaseType.Get();
            if (baseTypeMember == null)
            {
                return false;
            }

            var clrBaseType = baseTypeMember.Bindings.FirstOrDefault<Type>();
            if (clrBaseType == null || !clrBaseType.IsGenericType || clrBaseType.IsGenericTypeDefinition)
            {
                return false;
            }

            return (clrBaseType.GetGenericTypeDefinition() == typeof(WebPage<>));
        }
    }
}