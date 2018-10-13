using System.Collections.Generic;
using System.Linq;
using MetaPrograms.Members;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MetaPrograms.Adapters.Roslyn.Writer.SyntaxEmitters
{
    public static class AttributeSyntaxEmitter
    {
        public static AttributeSyntax EmitSyntax(AttributeDescription description)
        {
            var syntax = SyntaxFactory.Attribute(SyntaxHelpers.GetTypeFullNameSyntax(description.AttributeType, stripSuffix: "Attribute"));

            if (description.ConstructorArguments.Count > 0 || description.PropertyValues.Count > 0)
            {
                syntax = syntax
                    .WithArgumentList(
                        SyntaxFactory.AttributeArgumentList(
                            SyntaxFactory.SeparatedList<AttributeArgumentSyntax>(
                                description.ConstructorArguments.Select(arg => 
                                    SyntaxFactory.AttributeArgument(SyntaxHelpers.GetLiteralSyntax(arg)))
                                .Concat(description.PropertyValues.Select(pv => 
                                    SyntaxFactory.AttributeArgument(ExpressionSyntaxEmitter.EmitSyntax(pv.Value))
                                        .WithNameEquals(SyntaxFactory.NameEquals(SyntaxFactory.IdentifierName(pv.Name)))
                                ))
                            )
                        )
                    );
            }

            return syntax;
        }

        public static SyntaxList<AttributeListSyntax> EmitSyntaxList(IEnumerable<AttributeDescription> attributes)
        {
            return SyntaxFactory.SingletonList<AttributeListSyntax>(
                SyntaxFactory.AttributeList(
                    SyntaxFactory.SeparatedList<AttributeSyntax>(
                        attributes.Select(EmitSyntax))));
        }
    }
}
