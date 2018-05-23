using System;
using System.Collections.Generic;
using System.Linq;
using MetaPrograms.CodeModel.Imperative;
using MetaPrograms.CodeModel.Imperative.Members;

namespace Example.WebUIModel.Metadata
{
    public class WebUIMetadata
    {
        private readonly ImmutableCodeModel _codeModel;
        //private readonly List<WebPageMetadata> _webPages;

        public WebUIMetadata(ImmutableCodeModel codeModel)
        {
            _codeModel = codeModel;
            DiscoverWebPages();
        }

        private void DiscoverWebPages()
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
            

            // IsInheritedFromWabPageClass(TypeMember type)
            //{
            //    var clrBase = type.BaseType.Bindings.OfType<System.Type>().SingleOrDefault();
                
            //    if (clrBase != null)
            //    {
            //        return (clrBase.IsGenericType && clrBase.GetGenericTypeDefinition() == typeof(WebPage<>));
            //    }
                
                
            //}
            
            
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