using MetaPrograms.Members;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MetaPrograms.CSharp.Writer.SyntaxEmitters
{
    internal class MethodSyntaxEmitter : MemberSyntaxEmitterBase<MethodMember, MethodDeclarationSyntax>
    {
        public MethodSyntaxEmitter(MethodMember method)
            : base(method)
        {
        }

        public override MethodDeclarationSyntax EmitSyntax()
        {
            TypeSyntax returnTypeSyntax = (Member.Signature.IsVoid
                ? SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword))
                : Member.Signature.ReturnValue.Type.GetTypeNameSyntax());

            OutputSyntax =
                SyntaxFactory.MethodDeclaration(
                    returnTypeSyntax,
                    SyntaxFactory.Identifier(Member.Name)
                );

            OutputSyntax = OutputSyntax.WithModifiers(EmitMemberModifiers());

            if (Member.Signature.Parameters.Count > 0)
            {
                OutputSyntax = OutputSyntax.WithParameterList(MethodSignatureSyntaxEmitter.EmitParameterListSyntax(Member.Signature));
            }

            if (Member.Attributes.Count > 0)
            {
                OutputSyntax = OutputSyntax.WithAttributeLists(EmitAttributeLists());
            }
            
            OutputSyntax = OutputSyntax.WithBody(Member.Body.ToSyntax());

            return OutputSyntax;
        }

        protected override SyntaxTokenList EmitMemberModifiers()
        {
            var baseList = base.EmitMemberModifiers();

            if (Member.Signature.IsAsync)
            {
                return baseList.Add(SyntaxFactory.Token(SyntaxKind.AsyncKeyword));
            }

            return baseList;
        }
    }
}