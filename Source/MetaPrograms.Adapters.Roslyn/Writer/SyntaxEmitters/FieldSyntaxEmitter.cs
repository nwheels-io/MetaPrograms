using System.Collections.Generic;
using System.Linq;
using MetaPrograms.CodeModel.Imperative.Members;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MetaPrograms.Adapters.Roslyn.Writer.SyntaxEmitters
{
    public class FieldSyntaxEmitter : MemberSyntaxEmitterBase<FieldMember, FieldDeclarationSyntax>
    {
        public FieldSyntaxEmitter(FieldMember field)
            : base(field)
        {
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public override FieldDeclarationSyntax EmitSyntax()
        {
            var declarator = SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier(Member.Name));

            if (Member.Initializer != null)
            {
                declarator = declarator.WithInitializer(SyntaxFactory.EqualsValueClause(ExpressionSyntaxEmitter.EmitSyntax(Member.Initializer)));
            }

            OutputSyntax =
                SyntaxFactory.FieldDeclaration(
                    SyntaxFactory.VariableDeclaration(
                        Member.Type.GetTypeNameSyntax()
                    )
                    .WithVariables(
                        SyntaxFactory.SingletonSeparatedList<VariableDeclaratorSyntax>(
                            declarator
                        )
                    )
                )
                .WithModifiers(
                    EmitMemberModifiers()
                );

            //TODO: emit attributes

            return OutputSyntax;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        protected override IEnumerable<SyntaxKind> GetMemberModifierKeywords()
        {
            if (Member.IsReadOnly)
            {
                return base.GetMemberModifierKeywords().Append(SyntaxKind.ReadOnlyKeyword);
            }

            return base.GetMemberModifierKeywords();
        }
    }
}