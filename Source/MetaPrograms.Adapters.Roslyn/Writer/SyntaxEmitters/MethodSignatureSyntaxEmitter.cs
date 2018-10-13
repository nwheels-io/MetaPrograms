using System.Linq;
using MetaPrograms.Members;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MetaPrograms.Adapters.Roslyn.Writer.SyntaxEmitters
{
    public static class MethodSignatureSyntaxEmitter
    {
        public static ParameterListSyntax EmitParameterListSyntax(MethodSignature signature)
        {
            return SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList<ParameterSyntax>(signature.Parameters.Select(EmitParameterSyntax)));
        }

        private static ParameterSyntax EmitParameterSyntax(MethodParameter parameter)
        {
            var syntax = SyntaxFactory.Parameter(SyntaxFactory.Identifier(parameter.Name));

            switch (parameter.Modifier)
            {
                case MethodParameterModifier.Ref:
                    syntax = syntax.WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.RefKeyword)));
                    break;
                case MethodParameterModifier.Out:
                    syntax = syntax.WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.OutKeyword)));
                    break;
            }

            if (parameter.Attributes.Count > 0)
            {
                syntax = syntax.WithAttributeLists(AttributeSyntaxEmitter.EmitSyntaxList(parameter.Attributes));
            }

            syntax = syntax.WithType(parameter.Type.GetTypeNameSyntax());

            return syntax;
        }
    }
}
