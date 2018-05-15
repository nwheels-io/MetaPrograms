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
        public TypeReaderMechanism(CodeModelBuilder modelBuilder, INamedTypeSymbol symbol)
        {
            ModelBuilder = modelBuilder;
            MemberBuilder = new TypeMemberBuilder();
            Symbol = symbol;
            FullyQualifiedMetadataName = Symbol.GetFullyQualifiedMetadataName();
            ClrType = Type.GetType(FullyQualifiedMetadataName, throwOnError: false);
            MemberBuilder.Status = MemberStatus.Incomplete;
            MemberRef = MemberBuilder.GetTemporaryProxy().GetRef();

            MemberBuilder.Bindings.Add(Symbol);
            MemberBuilder.Bindings.Add(FullyQualifiedMetadataName);

            if (ClrType != null)
            {
                MemberBuilder.Bindings.Add(ClrType);
            }

        }

        public CodeModelBuilder ModelBuilder { get; }
        public TypeMemberBuilder MemberBuilder { get; }
        public INamedTypeSymbol Symbol { get; }
        public string FullyQualifiedMetadataName { get; }
        public Type ClrType { get; }
        public bool IsTopLevelType => (Symbol.ContainingSymbol is INamespaceSymbol);
        public MemberRef<TypeMember> MemberRef { get; }
        public RealTypeMember FinalType { get; private set; }

        public void RegisterTemporaryProxy()
        {
            ModelBuilder.RegisterMember(MemberRef.AsRef<AbstractMember>(), IsTopLevelType);
        }

        public void RegisterFinalType()
        {
            MemberBuilder.Status = MemberStatus.Compiled;
            FinalType = new RealTypeMember(MemberBuilder);
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
                MemberBuilder.GenericParameters.AddRange(
                    Symbol.TypeParameters.Select(
                        p => ModelBuilder.GetMember<TypeMember, ISymbol>(p)));
            }
        }

        public void ReadBaseType()
        {
            MemberBuilder.BaseType = (
                Symbol.BaseType == null || Symbol.BaseType.SpecialType == SpecialType.System_Object
                    ? MemberRef<TypeMember>.Null
                    : ModelBuilder.GetMember<TypeMember, ISymbol>(Symbol.BaseType));
        }

        public void ReadBaseInterfaces()
        {
            foreach (var interfaceSymbol in Symbol.Interfaces)
            {
                MemberBuilder.Interfaces.Add(ModelBuilder.GetMember<TypeMember, ISymbol>(interfaceSymbol));
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

        //public TypeMember CreateTypeMemberWithBindings(bool shouldCreateProxy)
        //{
        //    var type = (
        //        shouldCreateProxy 
        //        ? new ProxyTypeMember(MemberBuilder) as TypeMember 
        //        : new RealTypeMember(MemberBuilder) as TypeMember);
            
        //    type.Bindings.Add(Symbol);
        //    type.Bindings.Add(FullyQualifiedMetadataName);

        //    if (ClrType != null)
        //    {
        //        type.Bindings.Add(ClrType);
        //    }

        //    return type;
        //}
    }
}