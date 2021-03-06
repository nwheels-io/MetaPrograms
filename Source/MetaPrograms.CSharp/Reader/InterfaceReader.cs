﻿using System;
using System.Linq;
using MetaPrograms;
using MetaPrograms.Members;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MetaPrograms.CSharp.Reader
{
    public class InterfaceReader : IPhasedTypeReader
    {
        private readonly TypeReaderMechanism _mechanism;

        public InterfaceReader(TypeReaderMechanism mechanism)
        {
            _mechanism = mechanism;
        }

        public void ReadName()
        {
            _mechanism.ReadName();
        }

        public void RegisterProxy()
        {
            _mechanism.RegisterTemporaryProxy();
        }

        public void ReadGenerics()
        {
            _mechanism.ReadGenerics();
        }

        public void ReadAncestors()
        {
            _mechanism.ReadContainingType();
            _mechanism.ReadBaseInterfaces();
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
