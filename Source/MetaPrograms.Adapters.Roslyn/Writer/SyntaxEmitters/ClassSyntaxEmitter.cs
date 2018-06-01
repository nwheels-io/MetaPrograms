using Microsoft.CodeAnalysis.CSharp.Syntax;
using MetaPrograms.CodeModel.Imperative.Expressions;
using MetaPrograms.CodeModel.Imperative.Members;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace NWheels.Compilation.Adapters.Roslyn.SyntaxEmitters
{
    public class ClassSyntaxEmitter : TypeMemberSyntaxEmitterBase<TypeMember, ClassDeclarationSyntax>
    {
        public ClassSyntaxEmitter(TypeMember member) 
            : base(member)
        {
        }

        public override ClassDeclarationSyntax EmitSyntax()
        {
            OutputSyntax = ClassDeclaration(Member.Name);

            if (Member.Attributes.Count > 0)
            {
                OutputSyntax = OutputSyntax.WithAttributeLists(EmitAttributeLists());
            }

            OutputSyntax = OutputSyntax.WithModifiers(EmitMemberModifiers());

            if (Member.BaseType.IsNotNull || Member.Interfaces.Count > 0)
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
