﻿using System;
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
        private readonly TypeReader _typeReader;

        public ClassReader(TypeReader typeReader)
        {
            _typeReader = typeReader;
        }

        public ClassReader(CodeModelBuilder modelBuilder, SemanticModel semanticModel, ClassDeclarationSyntax syntax)
            : this(new TypeReader(modelBuilder, semanticModel, syntax))
        {
        }

        public TypeMember Read()
        {
            _typeReader.MemberBuilder.TypeKind = TypeMemberKind.Class;
            
            _typeReader.ReadName();
            _typeReader.RegisterIncompleteTypeMember();

            _typeReader.ReadGenerics();
            _typeReader.ReadBaseType();
            _typeReader.ReadBaseInterfaces();
            _typeReader.ReadMemberDeclarations();
            _typeReader.ReadMemberImplementations();
            
            return _typeReader.RegisterCompleteTypeMember();
        }

        public TypeReader TypeReader => _typeReader;
    }
}
