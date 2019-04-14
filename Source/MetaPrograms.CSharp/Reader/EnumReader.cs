using System;
using System.Collections.Generic;
using System.Text;
using MetaPrograms;
using MetaPrograms.Members;
using Microsoft.CodeAnalysis;

namespace MetaPrograms.CSharp.Reader
{
    public class EnumReader : IPhasedTypeReader
    {
        private readonly TypeReaderMechanism _mechanism;

        public EnumReader(TypeReaderMechanism mechanism)
        {
            _mechanism = mechanism;
            _mechanism.MemberBuilder.TypeKind = TypeMemberKind.Enum;
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
        }

        public void ReadAncestors()
        {
            _mechanism.ReadContainingType();
        }

        public void ReadMemberDeclarations()
        {
            _mechanism.ReadMemberDeclarations();
        }

        public void ReadAttributes()
        {
            _mechanism.ReadAttributes();
        }

        public void ReadMemberImplementations()
        {
        }

        public void RegisterReal()
        {
            _mechanism.RegisterFinalType();
        }

        public INamedTypeSymbol TypeSymbol => _mechanism.Symbol;
        public TypeMember TypeMember => _mechanism.CurrentMember;
    }
}
