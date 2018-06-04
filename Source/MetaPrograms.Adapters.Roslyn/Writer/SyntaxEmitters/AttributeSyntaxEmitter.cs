using Microsoft.CodeAnalysis.CSharp.Syntax;
using MetaPrograms.CodeModel.Imperative.Members;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace NWheels.Compilation.Adapters.Roslyn.SyntaxEmitters
{
    public static class AttributeSyntaxEmitter
    {
        public static AttributeSyntax EmitSyntax(AttributeDescription description)
        {
            var syntax = Attribute(SyntaxHelpers.GetTypeFullNameSyntax(description.AttributeType, stripSuffix: "Attribute"));

            if (description.ConstructorArguments.Count > 0 || description.PropertyValues.Count > 0)
            {
                syntax = syntax
                    .WithArgumentList(
                        AttributeArgumentList(
                            SeparatedList<AttributeArgumentSyntax>(
                                description.ConstructorArguments.Select(arg => 
                                    AttributeArgument(SyntaxHelpers.GetLiteralSyntax(arg)))
                                .Concat(description.PropertyValues.Select(pv => 
                                    AttributeArgument(ExpressionSyntaxEmitter.EmitSyntax(pv.Value))
                                        .WithNameEquals(NameEquals(IdentifierName(pv.Name)))
                                ))
                            )
                        )
                    );
            }

            return syntax;
        }

        public static SyntaxList<AttributeListSyntax> EmitSyntaxList(IEnumerable<AttributeDescription> attributes)
        {
            return SingletonList<AttributeListSyntax>(
                AttributeList(
                    SeparatedList<AttributeSyntax>(
                        attributes.Select(EmitSyntax))));
        }
    }
}
