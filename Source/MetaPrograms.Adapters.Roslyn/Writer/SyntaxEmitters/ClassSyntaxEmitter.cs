using MetaPrograms.CodeModel.Imperative.Members;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MetaPrograms.Adapters.Roslyn.Writer.SyntaxEmitters
{
    public class ClassSyntaxEmitter : TypeMemberSyntaxEmitterBase<TypeMember, ClassDeclarationSyntax>
    {
        public ClassSyntaxEmitter(TypeMember member) 
            : base(member)
        {
        }

        public override ClassDeclarationSyntax EmitSyntax()
        {
            OutputSyntax = SyntaxFactory.ClassDeclaration(Member.Name.ToPascalCase());

            if (Member.Attributes.Count > 0)
            {
                OutputSyntax = OutputSyntax.WithAttributeLists(EmitAttributeLists());
            }

            OutputSyntax = OutputSyntax.WithModifiers(EmitMemberModifiers());

            if (Member.BaseType != null || Member.Interfaces.Count > 0)
            {
                OutputSyntax = OutputSyntax.WithBaseList(EmitBaseList());
            }

            if (Member.IsGenericDefinition && Member.GenericParameters.Count > 0)
            {
                OutputSyntax = OutputSyntax.WithTypeParameterList(EmitTypeParameterList());
            }

            OutputSyntax = OutputSyntax.WithMembers(EmitMembers());

            return OutputSyntax;
        }
    }
}
