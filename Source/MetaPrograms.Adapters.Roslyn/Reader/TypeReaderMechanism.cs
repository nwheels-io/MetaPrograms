using System;
using System.Linq;
using MetaPrograms.CodeModel.Imperative;
using MetaPrograms.CodeModel.Imperative.Members;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MetaPrograms.Adapters.Roslyn.Reader
{
    public class TypeReaderMechanism
    {
        public TypeReaderMechanism(CodeModelBuilder modelBuilder, SemanticModel semanticModel, TypeDeclarationSyntax syntax)
            : this(modelBuilder, semanticModel, semanticModel.GetDeclaredSymbol(syntax))
        {
            Syntax = syntax;
        }

        public TypeReaderMechanism(CodeModelBuilder modelBuilder, SemanticModel semanticModel, INamedTypeSymbol symbol)
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
        public ProxyTypeMember ProxyType { get; private set; }
        public RealTypeMember RealType { get; private set; }

        public void RegisterProxyType()
        {
            MemberBuilder.Status = MemberStatus.Incomplete;
            this.ProxyType = (ProxyTypeMember)CreateTypeMemberWithBindings(shouldCreateProxy: true);
            ModelBuilder.RegisterMember(ProxyType);
        }

        public void RegisterRealType()
        {
            MemberBuilder.Status = MemberStatus.Compiled;
            this.RealType = (RealTypeMember)CreateTypeMemberWithBindings(shouldCreateProxy: false);

            if (ProxyType != null)
            {
                ProxyType.AssignRealTypeOnce(RealType);
            }

            if (IsTopLevelType)
            {
                ModelBuilder.RegisterTopLevelMember(RealType);
            }
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

        public TypeMember CreateTypeMemberWithBindings(bool shouldCreateProxy)
        {
            var type = (
                shouldCreateProxy 
                ? new ProxyTypeMember(MemberBuilder) as TypeMember 
                : new RealTypeMember(MemberBuilder) as TypeMember);
            
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