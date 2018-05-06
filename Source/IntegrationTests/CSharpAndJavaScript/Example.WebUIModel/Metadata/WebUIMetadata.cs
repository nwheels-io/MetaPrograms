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
        }

        private void DiscoverWebPages()
        {
            var topLevelClasses = _codeModel.TopLevelMembers
                .OfType<TypeMember>()
                .Where(t => t.TypeKind == TypeMemberKind.Class);
            
            

            // IsInheritedFromWabPageClass(TypeMember type)
            //{
            //    var clrBase = type.BaseType.Bindings.OfType<System.Type>().SingleOrDefault();
                
            //    if (clrBase != null)
            //    {
            //        return (clrBase.IsGenericType && clrBase.GetGenericTypeDefinition() == typeof(WebPage<>));
            //    }
                
                
            //}
            
            
        }
        
        
        
    }
}