using System;
using System.Collections.Generic;
using System.Text;
using MetaPrograms.CodeModel.Imperative;
using MetaPrograms.CodeModel.Imperative.Members;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MetaPrograms.Adapters.Roslyn.Writer
{
    public class TypeMemberSyntaxBuilder
    {
        private readonly ImperativeCodeModel _codeModel;
        private readonly TypeMember _type;

        public TypeMemberSyntaxBuilder(ImperativeCodeModel codeModel, TypeMember type)
        {
            _codeModel = codeModel;
            _type = type;
        }

        public SyntaxTree BuildSyntaxTree()
        {
            var syntax = CompilationUnit()
                .WithMembers(
                    SingletonList<MemberDeclarationSyntax>(
                        NamespaceDeclaration(
                                QualifiedName(
                                    IdentifierName("A"),
                                    IdentifierName("B")))
                            .WithMembers(SingletonList<MemberDeclarationSyntax>(ClassDeclaration("C")))))
                .NormalizeWhitespace();

            return syntax.SyntaxTree;
        }
    }
}
