using System.Linq;
using MetaPrograms.Members;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MetaPrograms.Adapters.Roslyn.Writer.SyntaxEmitters
{
    public class EnumSyntaxEmitter : MemberSyntaxEmitterBase<TypeMember, EnumDeclarationSyntax>
    {
        public EnumSyntaxEmitter(TypeMember member)
            : base(member)
        {
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public override EnumDeclarationSyntax EmitSyntax()
        {
            OutputSyntax = SyntaxFactory.EnumDeclaration(Member.Name);

            if (Member.Attributes.Count > 0)
            {
                OutputSyntax = OutputSyntax.WithAttributeLists(EmitAttributeLists());
            }

            OutputSyntax = OutputSyntax.WithModifiers(EmitMemberModifiers());
            OutputSyntax = OutputSyntax.WithMembers(EmitEnumMembers());

            return OutputSyntax;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private SeparatedSyntaxList<EnumMemberDeclarationSyntax> EmitEnumMembers()
        {
            return SyntaxFactory.SeparatedList(
                Member.Members.OfType<EnumMember>()
                    .Select(ToEnumMemberSyntax));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private EnumMemberDeclarationSyntax ToEnumMemberSyntax(EnumMember member)
        {
            var syntax = SyntaxFactory.EnumMemberDeclaration(SyntaxFactory.Identifier(member.Name));

            if (member.Value != null)
            {
                syntax = syntax.WithEqualsValue(SyntaxFactory.EqualsValueClause(SyntaxHelpers.GetLiteralSyntax(member.Value)));
            }

            return syntax;
        }
    }
}
