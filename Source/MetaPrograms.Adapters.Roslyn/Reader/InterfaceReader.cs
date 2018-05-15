using System;
using System.Linq;
using MetaPrograms.CodeModel.Imperative;
using MetaPrograms.CodeModel.Imperative.Members;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MetaPrograms.Adapters.Roslyn.Reader
{
    public class InterfaceReader
    {
        private readonly TypeReader _typeReader;

        public InterfaceReader(TypeReader typeReader)
        {
            _typeReader = typeReader;
        }

        public InterfaceReader(CodeModelBuilder modelBuilder, SemanticModel semanticModel, InterfaceDeclarationSyntax syntax)
            : this(new TypeReader(modelBuilder, semanticModel, syntax))
        {
        }

        public TypeMember Read()
        {
            _typeReader.MemberBuilder.TypeKind = TypeMemberKind.Interface;
            
            _typeReader.ReadName();
            _typeReader.RegisterIncompleteTypeMember();

            _typeReader.ReadGenerics();
            _typeReader.ReadBaseInterfaces();
            _typeReader.ReadMemberDeclarations();
            
            return _typeReader.RegisterCompleteTypeMember();
        }

        public TypeReader TypeReader => _typeReader;
    }
}
