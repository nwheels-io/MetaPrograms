using System;
using System.Linq;
using MetaPrograms.CodeModel.Imperative;
using MetaPrograms.CodeModel.Imperative.Members;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MetaPrograms.Adapters.Roslyn.Reader
{
    public class InterfaceReader : IPhasedTypeReader
    {
        private readonly TypeReaderMechanism _mechanism;

        public InterfaceReader(TypeReaderMechanism mechanism)
        {
            _mechanism = mechanism;
        }

        public TypeMember Read()
        {
            _mechanism.MemberBuilder.TypeKind = TypeMemberKind.Interface;
            
            _mechanism.ReadName();
            _mechanism.RegisterTemporaryProxy();

            _mechanism.ReadGenerics();
            _mechanism.ReadBaseInterfaces();
            _mechanism.ReadMemberDeclarations();
            
            _mechanism.RegisterFinalType();
            return _mechanism.FinalType;
        }

        public TypeReaderMechanism Mechanism => _mechanism;

        public void ReadName()
        {
            throw new NotImplementedException();
        }

        public void RegisterProxy()
        {
            throw new NotImplementedException();
        }

        public void ReadGenerics()
        {
            throw new NotImplementedException();
        }

        public void ReadAncestors()
        {
            throw new NotImplementedException();
        }

        public void ReadMemberDeclarations()
        {
            throw new NotImplementedException();
        }

        public void ReadAttributes()
        {
            throw new NotImplementedException();
        }

        public void ReadMemberImplementations()
        {
            throw new NotImplementedException();
        }

        public void RegisterReal()
        {
            throw new NotImplementedException();
        }

        public TypeMember TypeMember { get; }
    }
}
