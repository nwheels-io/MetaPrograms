using System.Collections.Generic;
using System.Linq;
using MetaPrograms.Members;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MetaPrograms.Adapters.Roslyn.Writer.SyntaxEmitters
{
    public abstract class MemberSyntaxEmitterBase<TMember, TSyntax> : SyntaxEmitterBase<TSyntax>
        where TMember : AbstractMember
        where TSyntax : MemberDeclarationSyntax
    {
        protected MemberSyntaxEmitterBase(TMember member)
        {
            this.Member = member;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public TMember Member { get; }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        protected SyntaxList<AttributeListSyntax> EmitAttributeLists()
        {
            return AttributeSyntaxEmitter.EmitSyntaxList(Member.Attributes);
            //return SingletonList<AttributeListSyntax>(
            //    AttributeList(
            //        SeparatedList<AttributeSyntax>(
            //            Member.Attributes.Select(AttributeSyntaxEmitter.EmitSyntax))));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        protected virtual IEnumerable<SyntaxKind> GetMemberModifierKeywords()
        {
            return _s_visibilityKeywords[Member.Visibility].Concat(_s_modifierKeywords[Member.Modifier]);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        protected virtual SyntaxTokenList EmitMemberModifiers()
        {
            return SyntaxFactory.TokenList(GetMemberModifierKeywords().Select(SyntaxFactory.Token));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        protected SyntaxTokenList EmitMemberVisibility()
        {
            return SyntaxFactory.TokenList(_s_visibilityKeywords[Member.Visibility].Select(SyntaxFactory.Token));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private static readonly IReadOnlyDictionary<MemberVisibility, SyntaxKind[]> _s_visibilityKeywords =
            new Dictionary<MemberVisibility, SyntaxKind[]> {
                [MemberVisibility.Public] = new[] { SyntaxKind.PublicKeyword },
                [MemberVisibility.Protected] = new[] { SyntaxKind.ProtectedKeyword },
                [MemberVisibility.Internal] = new[] { SyntaxKind.InternalKeyword },
                [MemberVisibility.InternalProtected] = new[] { SyntaxKind.InternalKeyword, SyntaxKind.ProtectedKeyword },
                [MemberVisibility.Private] = new[] { SyntaxKind.PrivateKeyword }
            };

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private static readonly IReadOnlyDictionary<MemberModifier, SyntaxKind[]> _s_modifierKeywords =
            new Dictionary<MemberModifier, SyntaxKind[]> {
                [MemberModifier.Abstract] = new[] { SyntaxKind.AbstractKeyword },
                [MemberModifier.Virtual] = new[] { SyntaxKind.VirtualKeyword },
                [MemberModifier.Override] = new[] { SyntaxKind.OverrideKeyword },
                [MemberModifier.Static] = new[] { SyntaxKind.StaticKeyword },
                [MemberModifier.None] = new SyntaxKind[0]
            };
    }
}
