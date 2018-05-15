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
        private readonly TypeReaderMechanism _typeReaderMechanism;

        public InterfaceReader(TypeReaderMechanism typeReaderMechanism)
        {
            _typeReaderMechanism = typeReaderMechanism;
        }

        public InterfaceReader(CodeModelBuilder modelBuilder, SemanticModel semanticModel, InterfaceDeclarationSyntax syntax)
            : this(new TypeReaderMechanism(modelBuilder, semanticModel, syntax))
        {
        }

        public TypeMember Read()
        {
            _typeReaderMechanism.MemberBuilder.TypeKind = TypeMemberKind.Interface;
            
            _typeReaderMechanism.ReadName();
            _typeReaderMechanism.RegisterProxyType();

            _typeReaderMechanism.ReadGenerics();
            _typeReaderMechanism.ReadBaseInterfaces();
            _typeReaderMechanism.ReadMemberDeclarations();
            
            _typeReaderMechanism.RegisterRealType();
            return _typeReaderMechanism.RealType;
        }

        public TypeReaderMechanism TypeReaderMechanism => _typeReaderMechanism;
    }
}
