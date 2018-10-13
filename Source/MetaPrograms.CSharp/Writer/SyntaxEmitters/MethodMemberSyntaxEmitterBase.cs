using MetaPrograms.Members;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MetaPrograms.CSharp.Writer.SyntaxEmitters
{
    public abstract class MethodMemberSyntaxEmitterBase<TMember, TSyntax> : MemberSyntaxEmitterBase<TMember, TSyntax>
        where TMember : MethodMemberBase
        where TSyntax : BaseMethodDeclarationSyntax
    {
        protected MethodMemberSyntaxEmitterBase(TMember member) 
            : base(member)
        {
        }


    }
}
