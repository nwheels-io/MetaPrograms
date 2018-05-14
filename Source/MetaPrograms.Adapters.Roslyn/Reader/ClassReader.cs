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
        private readonly CodeModelBuilder _modelBuilder;
        private readonly TypeMemberBuilder _memberBuilder;
        private readonly SemanticModel _semanticModel;
        private readonly ClassDeclarationSyntax _syntax;
        private readonly INamedTypeSymbol _symbol;
        private readonly string _fullyQualifiedMetadataName;
        private readonly Type _clrType;
        
        public ClassReader(CodeModelBuilder modelBuilder, SemanticModel semanticModel, ClassDeclarationSyntax syntax)
        {
            _modelBuilder = modelBuilder;
            _memberBuilder = new TypeMemberBuilder();
            _semanticModel = semanticModel;
            _syntax = syntax;
            _symbol = semanticModel.GetDeclaredSymbol(syntax);
            _fullyQualifiedMetadataName = _symbol.GetFullyQualifiedMetadataName();
            _clrType = Type.GetType(_fullyQualifiedMetadataName, throwOnError: false);
        }

        public ClassReader(CodeModelBuilder modelBuilder, SemanticModel semanticModel, INamedTypeSymbol symbol)
        {
            _modelBuilder = modelBuilder;
            _memberBuilder = new TypeMemberBuilder();
            _semanticModel = semanticModel;
            _symbol = symbol;
            _syntax = symbol.DeclaringSyntaxReferences.OfType<ClassDeclarationSyntax>().FirstOrDefault();
            _fullyQualifiedMetadataName = _symbol.GetFullyQualifiedMetadataName();
            _clrType = Type.GetType(_fullyQualifiedMetadataName, throwOnError: false);
        }

        public TypeMember Read()
        {
            ReadName();
            RegisterIncompleteTypeMember();

            ReadGenerics();
            ReadBaseTypes();
            ReadMembers();
            
            return RegisterCompleteTypeMember();
        }

        private void RegisterIncompleteTypeMember()
        {
            _memberBuilder.Status = MemberStatus.Incomplete;
            var incompleteMember = CreateTypeMemberWithBindings();
            _modelBuilder.RegisterMember(incompleteMember, isTopLevel: false);
        }

        private TypeMember RegisterCompleteTypeMember()
        {
            _memberBuilder.Status = MemberStatus.Compiled;
            var completeMember = CreateTypeMemberWithBindings();
            _modelBuilder.RegisterMember(completeMember, isTopLevel: _symbol.ContainingSymbol is INamespaceSymbol);

            return completeMember;
        }

        private void ReadName()
        {
            _memberBuilder.Namespace = _symbol.GetFullNamespaceName();
            _memberBuilder.Name = _symbol.Name;
            _memberBuilder.TypeKind = TypeMemberKind.Class;
        }

        private void ReadGenerics()
        {
            _memberBuilder.IsGenericType = _symbol.IsGenericType;
            _memberBuilder.IsGenericDefinition = _symbol.IsDefinition;
            
            if (_symbol.IsGenericType)
            {
                _memberBuilder.GenericTypeParameters.AddRange(
                    _symbol.TypeParameters.Select(
                        p => _modelBuilder.IncludeType(p, _semanticModel)));
            }
        }

        private void ReadBaseTypes()
        {
            _memberBuilder.BaseType = (
                _symbol.BaseType == null || _symbol.BaseType.SpecialType == SpecialType.System_Object
                    ? null
                    : _modelBuilder.IncludeType(_symbol.BaseType, _semanticModel));

            foreach (var interfaceSymbol in _symbol.Interfaces)
            {
                _memberBuilder.Interfaces.Add(_modelBuilder.IncludeType(interfaceSymbol, _semanticModel));
            }
        }

        private void ReadMembers()
        {
            throw new NotImplementedException();
        }

        private TypeMember CreateTypeMemberWithBindings()
        {
            var type = new TypeMember(_memberBuilder);
            
            type.Bindings.Add(_symbol);
            type.Bindings.Add(_fullyQualifiedMetadataName);

            if (_clrType != null)
            {
                type.Bindings.Add(_clrType);
            }

            return type;
        }
    }
}
