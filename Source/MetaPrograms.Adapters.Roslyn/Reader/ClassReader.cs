using System;
using System.Linq;
using MetaPrograms.CodeModel.Imperative;
using MetaPrograms.CodeModel.Imperative.Members;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MetaPrograms.Adapters.Roslyn.Reader
{
    public class ClassReader
    {
        private readonly TypeReaderMechanism _typeReaderMechanism;

        public ClassReader(TypeReaderMechanism typeReaderMechanism)
        {
            _typeReaderMechanism = typeReaderMechanism;
        }

        public ClassReader(CodeModelBuilder modelBuilder, SemanticModel semanticModel, ClassDeclarationSyntax syntax)
            : this(new TypeReaderMechanism(modelBuilder, semanticModel, syntax))
        {
        }

        public TypeMember Read()
        {
            _typeReaderMechanism.MemberBuilder.TypeKind = TypeMemberKind.Class;
            
            _typeReaderMechanism.ReadName();
            _typeReaderMechanism.RegisterProxyType();

            _typeReaderMechanism.ReadGenerics();
            _typeReaderMechanism.ReadBaseType();
            _typeReaderMechanism.ReadBaseInterfaces();
            _typeReaderMechanism.ReadMemberDeclarations();
            _typeReaderMechanism.ReadMemberImplementations();
            
            _typeReaderMechanism.RegisterRealType();
            return _typeReaderMechanism.RealType;
        }

        public TypeReaderMechanism TypeReaderMechanism => _typeReaderMechanism;
    }
}
