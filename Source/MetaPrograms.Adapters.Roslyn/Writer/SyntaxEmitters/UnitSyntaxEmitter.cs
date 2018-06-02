using System;
using System.Collections.Generic;
using System.Text;
using MetaPrograms.Adapters.Roslyn.Writer.SyntaxEmitters;
using MetaPrograms.CodeModel.Imperative;
using MetaPrograms.CodeModel.Imperative.Members;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MetaPrograms.Adapters.Roslyn.Writer
{
    public class UnitSyntaxEmitter
    {
        private readonly ImperativeCodeModel _codeModel;
        private readonly TypeMember _type;

        public UnitSyntaxEmitter(ImperativeCodeModel codeModel, TypeMember type)
        {
            _codeModel = codeModel;
            _type = type;
        }

        public SyntaxTree BuildSyntaxTree()
        {
            var syntax = CompilationUnit()
                .WithMembers(
                    SingletonList<MemberDeclarationSyntax>(
                        NamespaceDeclaration(CreateQualifiedNameSyntax(_type.Namespace.Split('.')))
                            .WithMembers(SingletonList(TypeSyntaxEmitter.GetSyntax(_type)))
                    )
                )
                .NormalizeWhitespace();

            return syntax.SyntaxTree;
        }

        private NameSyntax CreateQualifiedNameSyntax(Span<string> parts)
        {
            switch (parts.Length)
            {
                case 0:
                    throw new ArgumentException("At least one name part must exist", nameof(parts));
                case 1:
                    return IdentifierName(parts[0]);
                default:
                    return QualifiedName(
                        CreateQualifiedNameSyntax(parts.Slice(0, parts.Length - 1)), 
                        IdentifierName(parts[parts.Length - 1])
                    );
            }
        }
    }
}
