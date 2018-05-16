using System;
using System.Collections.Generic;
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
        private readonly List<IPhasedMemberReader> _memberReaders = new List<IPhasedMemberReader>();
        private readonly Dictionary<ISymbol, IPhasedMemberReader> _memberReaderBySymbol = new Dictionary<ISymbol, IPhasedMemberReader>();

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
        public RealTypeMember FinalMember { get; private set; }
        public TypeMember CurrentMember => MemberRef.Get();

        public void RegisterTemporaryProxy()
        {
            ModelBuilder.RegisterMember(MemberRef.AsRef<AbstractMember>(), IsTopLevelType);
        }

        public void RegisterFinalType()
        {
            MemberBuilder.Status = MemberStatus.Compiled;
            FinalMember = new RealTypeMember(MemberBuilder);
            MemberBuilder.GetMemberSelfReference().ReassignOnce(FinalMember);
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
            foreach (var memberSymbol in Symbol.GetMembers().Where(m => !(m is INamedTypeSymbol)))
            {
                if (TryCreateMemberReader(memberSymbol, out IPhasedMemberReader memberReader))
                {
                    _memberReaderBySymbol.Add(memberSymbol, memberReader);
                    _memberReaders.Add(memberReader);
                }
            }

            _memberReaders.ForEach(m => m.ReadDeclaration());
        }

        public void ReadMemberImplementations()
        {
            //throw new NotImplementedException();
        }

        private bool TryCreateMemberReader(ISymbol memberSymbol, out IPhasedMemberReader memberReader)
        {
            if (memberSymbol is IFieldSymbol field)
            {
                memberReader = new FieldReader(field);
            }
            else
            {
                memberReader = null;
                return false;
            }

            return true;
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