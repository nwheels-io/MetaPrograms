using MetaPrograms.Members;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MetaPrograms.CSharp.Writer.SyntaxEmitters
{
    public class PropertySyntaxEmitter : MemberSyntaxEmitterBase<PropertyMember, PropertyDeclarationSyntax>
    {
        public PropertySyntaxEmitter(PropertyMember property)
            : base(property)
        {
        }

        public override PropertyDeclarationSyntax EmitSyntax()
        {
            OutputSyntax = SyntaxFactory.PropertyDeclaration(
                Member.PropertyType.GetTypeNameSyntax(),
                SyntaxFactory.Identifier(Member.Name)
            );

            OutputSyntax = OutputSyntax.WithModifiers(EmitMemberModifiers());

            OutputSyntax = OutputSyntax.WithAccessorList(
                SyntaxFactory.AccessorList(
                    SyntaxFactory.List<AccessorDeclarationSyntax>(new[] {
                        SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)                    
                            .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                        SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                            .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                    })
                )
            );

            return OutputSyntax;
        }
    }
}