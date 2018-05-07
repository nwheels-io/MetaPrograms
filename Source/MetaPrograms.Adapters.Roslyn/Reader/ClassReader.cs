using System;
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

        public void Read()
        {
            ReadName();
            RegisterIncompleteTypeMember();

            ReadGenerics();
            ReadBaseTypes();
            ReadMembers();
            
            RegisterCompleteTypeMember();
        }

        private void RegisterIncompleteTypeMember()
        {
            _memberBuilder.Status = MemberStatus.Incomplete;
            var incompleteMember = CreateTypeMemberWithBindings();
            _modelBuilder.RegisterMember(incompleteMember, isTopLevel: false);
        }

        private void RegisterCompleteTypeMember()
        {
            _memberBuilder.Status = MemberStatus.Compiled;
            var completeMember = CreateTypeMemberWithBindings();
            _modelBuilder.RegisterMember(completeMember, isTopLevel: _symbol.ContainingSymbol is INamespaceSymbol);
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
            //_memberBuilder.IsGenericTypeDefinition = _symbol.
            
            if (_symbol.IsGenericType)
            {
                
            }
        }

        private void ReadBaseTypes()
        {
            throw new NotImplementedException();
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
