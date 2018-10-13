#if false

using MetaPrograms.Members;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MetaPrograms.CSharp.Writer
{
    public class ClassSyntaxBuilder
    {
        public static ClassDeclarationSyntax GetSyntax(TypeMember type)
        {
            var syntax = ClassDeclaration(type.Name)
                .WithModifiers(MemberSyntaxBuilder.GetModifierSyntax(type))
                .WithAttributeLists(MemberSyntaxBuilder.GetAttributeListSyntax(type));

            return syntax;
        }
    }
}

#endif