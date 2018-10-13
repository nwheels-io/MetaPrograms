using System;
using System.Linq;
using MetaPrograms;
using MetaPrograms.Members;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MetaPrograms.Adapters.Roslyn.Reader
{
    public class StructReader : IPhasedTypeReader
    {
        private readonly TypeReaderMechanism _mechanism;

        public StructReader(TypeReaderMechanism mechanism)
        {
            _mechanism = mechanism;
            _mechanism.MemberBuilder.TypeKind = TypeMemberKind.Struct;
        }

        public void RegisterProxy()
        {
            _mechanism.RegisterTemporaryProxy();
        }

        public void ReadName()
        {
            _mechanism.ReadName();
        }

        public void ReadGenerics()
        {
            _mechanism.ReadGenerics();
        }

        public void ReadAncestors()
        {
            _mechanism.ReadContainingType();
            _mechanism.ReadBaseType();
            _mechanism.ReadBaseInterfaces();
        }

        public void ReadMemberDeclarations()
        {
            _mechanism.ReadMemberDeclarations();
        }

        public void ReadAttributes()
        {
            //throw new NotImplementedException();
        }

        public void ReadMemberImplementations()
        {
            //throw new NotImplementedException();

        }

        public void RegisterReal()
        {
            _mechanism.RegisterFinalType();
        }

        public INamedTypeSymbol TypeSymbol => _mechanism.Symbol;
        public TypeMember TypeMember => _mechanism.CurrentMember;
    }
}
