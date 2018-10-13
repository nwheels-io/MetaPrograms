#if false

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using MetaPrograms.Members;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MetaPrograms.Adapters.Roslyn.Writer
{
    public static class MemberSyntaxBuilder
    {
        public static MemberDeclarationSyntax GetSyntax(AbstractMember member)
        {
            if (member is TypeMember type)
            {
                return GetTypeSyntax(type);
            }
            
            throw new NotImplementedException();
        }
        
        public static TypeDeclarationSyntax GetTypeSyntax(TypeMember type)
        {
            switch (type.TypeKind)
            {
                case TypeMemberKind.Class:
                    return ClassSyntaxBuilder.GetSyntax(type);
                default:
                    throw new NotImplementedException();
            }
        }

        public static SyntaxTokenList GetModifierSyntax(AbstractMember member)
        {
            var syntaxList = new List<SyntaxToken>();

            AddVisibilitySyntax(member, syntaxList);
            AddModifierSyntax(member, syntaxList);
           
            return TokenList(syntaxList);
        }

        public static SyntaxList<AttributeListSyntax> GetAttributeListSyntax(AbstractMember member)
        {
            return List(
                member.Attributes.Select(attr =>
                    AttributeList(
                        SingletonSeparatedList(
                            GetAttributeSyntax(attr)
                        )
                    )
                )
            );
        }

        private static AttributeSyntax GetAttributeSyntax(AttributeDescription attribute)
        {
            var syntax = Attribute(IdentifierName(attribute.AttributeType.Get().Name))
                .WithArgumentList(
                    AttributeArgumentList(
                        SeparatedList<AttributeArgumentSyntax>(
                            attribute.ConstructorArguments.Select(arg => AttributeArgument(
                                ExpressionSyntaxBuilder.GetSyntax(arg)
                            ))
                            .Concat(attribute.PropertyValues.Select(propValue => 
                                AttributeArgument(ExpressionSyntaxBuilder.GetSyntax(propValue.Value))
                                    .WithNameEquals(NameEquals(IdentifierName(propValue.Name)))        
                            ))
                        )
                    )
                );

            return syntax;
        }

        private static void AddVisibilitySyntax(AbstractMember member, List<SyntaxToken> syntax)
        {
            switch (member.Visibility)
            {
                case MemberVisibility.Public:
                    syntax.Add(Token(SyntaxKind.PublicKeyword));
                    break;
                case MemberVisibility.Private:
                    syntax.Add(Token(SyntaxKind.PrivateKeyword));
                    break;
                case MemberVisibility.Protected:
                    syntax.Add(Token(SyntaxKind.ProtectedKeyword));
                    break;
                case MemberVisibility.Internal:
                    syntax.Add(Token(SyntaxKind.InternalKeyword));
                    break;
                case MemberVisibility.InternalProtected:
                    syntax.Add(Token(SyntaxKind.InternalKeyword));
                    syntax.Add(Token(SyntaxKind.ProtectedKeyword));
                    break;
                case MemberVisibility.PrivateProtected:
                    syntax.Add(Token(SyntaxKind.PrivateKeyword));
                    syntax.Add(Token(SyntaxKind.ProtectedKeyword));
                    break;
            }
        }

        private static void AddModifierSyntax(AbstractMember member, List<SyntaxToken> syntax)
        {
            switch (member.Modifier)
            {
                case MemberModifier.Abstract:
                    syntax.Add(Token(SyntaxKind.PublicKeyword));
                    break;
                case MemberModifier.Override:
                    syntax.Add(Token(SyntaxKind.OverrideKeyword));
                    break;
                case MemberModifier.Static:
                    syntax.Add(Token(SyntaxKind.StaticKeyword));
                    break;
                case MemberModifier.Virtual:
                    syntax.Add(Token(SyntaxKind.VirtualKeyword));
                    break;
            }
        }
    }
}

#endif