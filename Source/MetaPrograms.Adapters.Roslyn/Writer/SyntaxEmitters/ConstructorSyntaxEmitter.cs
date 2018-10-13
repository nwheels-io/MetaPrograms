using MetaPrograms.Members;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MetaPrograms.Adapters.Roslyn.Writer.SyntaxEmitters
{
    public class ConstructorSyntaxEmitter : MethodMemberSyntaxEmitterBase<ConstructorMember, ConstructorDeclarationSyntax>
    {
        public ConstructorSyntaxEmitter(ConstructorMember constructor) 
            : base(constructor)
        {
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public override ConstructorDeclarationSyntax EmitSyntax()
        {
            OutputSyntax = SyntaxFactory.ConstructorDeclaration(SyntaxFactory.Identifier(Member.DeclaringType.Name.ToPascalCase()));

            if (Member.Modifier != MemberModifier.Static)
            {
                OutputSyntax = OutputSyntax.WithModifiers(EmitMemberVisibility());

                if (Member.Signature.Parameters.Count > 0)
                {
                    OutputSyntax = OutputSyntax.WithParameterList(MethodSignatureSyntaxEmitter.EmitParameterListSyntax(Member.Signature));
                }
            }
            else
            {
                OutputSyntax = OutputSyntax.WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.StaticKeyword)));
            }
            
            if (Member.Attributes.Count > 0)
            {
                OutputSyntax = OutputSyntax.WithAttributeLists(EmitAttributeLists());
            }

            if (Member.CallThisConstructor != null)
            {
                OutputSyntax = OutputSyntax.WithInitializer(
                    SyntaxFactory.ConstructorInitializer(
                        SyntaxKind.ThisConstructorInitializer,
                        Member.CallThisConstructor.GetArgumentListSyntax()));
            }
            else if (Member.CallBaseConstructor != null)
            {
                OutputSyntax = OutputSyntax.WithInitializer(
                    SyntaxFactory.ConstructorInitializer(
                        SyntaxKind.BaseConstructorInitializer,
                        Member.CallBaseConstructor.GetArgumentListSyntax()));
            }

            OutputSyntax = OutputSyntax.WithBody(Member.Body.ToSyntax());

            return OutputSyntax;
        }
    }
}