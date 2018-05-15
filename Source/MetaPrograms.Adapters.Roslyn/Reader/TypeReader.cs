using System;
using System.Linq;
using MetaPrograms.CodeModel.Imperative;
using MetaPrograms.CodeModel.Imperative.Members;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MetaPrograms.Adapters.Roslyn.Reader
{
    public class TypeReader
    {
        public TypeReader(CodeModelBuilder modelBuilder, SemanticModel semanticModel, TypeDeclarationSyntax syntax)
            : this(modelBuilder, semanticModel, semanticModel.GetDeclaredSymbol(syntax))
        {
            Syntax = syntax;
        }

        public TypeReader(CodeModelBuilder modelBuilder, SemanticModel semanticModel, INamedTypeSymbol symbol)
        {
            ModelBuilder = modelBuilder;
            MemberBuilder = new TypeMemberBuilder();
            SemanticModel = semanticModel;
            Symbol = symbol;
            FullyQualifiedMetadataName = Symbol.GetFullyQualifiedMetadataName();
            ClrType = Type.GetType(FullyQualifiedMetadataName, throwOnError: false);
        }

        public CodeModelBuilder ModelBuilder { get; }
        public TypeMemberBuilder MemberBuilder { get; }
        public SemanticModel SemanticModel { get; }
        public TypeDeclarationSyntax Syntax { get; }
        public INamedTypeSymbol Symbol { get; }
        public string FullyQualifiedMetadataName { get; }
        public Type ClrType { get; }
        public bool IsTopLevelType => (Symbol.ContainingSymbol is INamespaceSymbol);
        
        public void RegisterIncompleteTypeMember()
        {
            MemberBuilder.Status = MemberStatus.Incomplete;
            var incompleteMember = CreateTypeMemberWithBindings();
            ModelBuilder.RegisterMember(incompleteMember, isTopLevel: false);
        }

        public TypeMember RegisterCompleteTypeMember()
        {
            MemberBuilder.Status = MemberStatus.Compiled;
            var completeMember = CreateTypeMemberWithBindings();
            ModelBuilder.RegisterMember(completeMember, IsTopLevelType);

            return completeMember;
        }

        public void ReadName()
        {
            MemberBuilder.Namespace = Symbol.GetFullNamespaceName();
            MemberBuilder.Name = Symbol.Name;
        }

        public void ReadGenerics()
        {
            MemberBuilder.IsGenericType = Symbol.IsGenericType;
            MemberBuilder.IsGenericDefinition = Symbol.IsDefinition;
            
            if (Symbol.IsGenericType)
            {
                MemberBuilder.GenericTypeParameters.AddRange(
                    Symbol.TypeParameters.Select(
                        p => ModelBuilder.IncludeType(p, SemanticModel)));
            }
        }

        public void ReadBaseType()
        {
            MemberBuilder.BaseType = (
                Symbol.BaseType == null || Symbol.BaseType.SpecialType == SpecialType.System_Object
                    ? null
                    : ModelBuilder.IncludeType(Symbol.BaseType, SemanticModel));
        }

        public void ReadBaseInterfaces()
        {
            foreach (var interfaceSymbol in Symbol.Interfaces)
            {
                MemberBuilder.Interfaces.Add(ModelBuilder.IncludeType(interfaceSymbol, SemanticModel));
            }
        }
        
        public void ReadMemberDeclarations()
        {
            //throw new NotImplementedException();
        }

        public void ReadMemberImplementations()
        {
            //throw new NotImplementedException();
        }

        public TypeMember CreateTypeMemberWithBindings()
        {
            var type = new TypeMember(MemberBuilder);
            
            type.Bindings.Add(Symbol);
            type.Bindings.Add(FullyQualifiedMetadataName);

            if (ClrType != null)
            {
                type.Bindings.Add(ClrType);
            }

            return type;
        }
    }
}