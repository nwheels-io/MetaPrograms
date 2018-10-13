using System.Linq;
using MetaPrograms.Members;

namespace Example.WebUIModel.Metadata
{
    public static class MemberExtensions
    {
        public static bool HasAttribute<TAttribute>(this AbstractMember member)
            where TAttribute : System.Attribute
        {
            return (TryGetAttribute<TAttribute>(member) != null);
        }

        public static AttributeDescription TryGetAttribute<TAttribute>(this AbstractMember member)
            where TAttribute : System.Attribute
        {
            return TryGetAttribute(member, typeof(TAttribute).FullName.Replace('+', '.'));
        }

        public static AttributeDescription TryGetAttribute(this AbstractMember member, string namespaceQualifiedTypeName)
        {
            return member.Attributes.FirstOrDefault(attr => attr.AttributeType.FullName == namespaceQualifiedTypeName);
        }
    }
}
