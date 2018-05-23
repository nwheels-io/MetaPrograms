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
            FullyQualifiedMetadataName = Symbol.GetSystemTypeMetadataName();
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
            MemberBuilder.GetMemberSelfReference().Reassign(FinalMember);
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
                MemberBuilder.GenericParameters.AddRange(Symbol.TypeParameters.Select(GetTypeParameterOrArgument));
                MemberBuilder.GenericArguments.AddRange(Symbol.TypeArguments.Select(GetTypeParameterOrArgument));
            }

            MemberRef<TypeMember> GetTypeParameterOrArgument(ITypeSymbol symbol)
            {
                if (symbol is INamedTypeSymbol argument)
                {
                    return ModelBuilder.TryGetMember<TypeMember>(argument);
                }
                else if (symbol is ITypeParameterSymbol parameter)
                {
                    return TypeParameterReader.Read(parameter).GetRef();
                }

                return MemberRef<TypeMember>.Null;
            }
        }

        public void ReadBaseType()
        {
            MemberBuilder.BaseType = (
                Symbol.BaseType == null || Symbol.BaseType.SpecialType == SpecialType.System_Object
                    ? MemberRef<TypeMember>.Null
                    : ModelBuilder.TryGetMember<TypeMember>(Symbol.BaseType));
        }

        public void ReadBaseInterfaces()
        {
            foreach (var interfaceSymbol in Symbol.Interfaces)
            {
                MemberBuilder.Interfaces.Add(ModelBuilder.TryGetMember<TypeMember>(interfaceSymbol));
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

            _memberReaders.ForEach(reader => {
                reader.ReadDeclaration();
                reader.Member.Bindings.Add(reader.Symbol);
                MemberBuilder.Members.Add(reader.Member.GetRefAsAbstract());
            });
        }

        public void ReadAttributes()
        {
            MemberBuilder.Attributes.AddRange(Symbol
                .GetAttributes()
                .Select(attr => AttributeReader.ReadAttribute(ModelBuilder, attr)));
        }

        public void ReadMemberImplementations()
        {
            //throw new NotImplementedException();
        }

        public void ReadEnumMembers()
        {
            var members = Symbol.GetMembers();
        }

        private bool TryCreateMemberReader(ISymbol memberSymbol, out IPhasedMemberReader memberReader)
        {
            if (memberSymbol is IFieldSymbol field)
            {
                memberReader = new FieldReader(ModelBuilder, field);
            }
            else if (memberSymbol is IMethodSymbol method && IsStandaloneMethodMember(method))
            {
                memberReader = CreateMethodOrConstructorReader(method);
            }
            else if (memberSymbol is IPropertySymbol property)
            {
                memberReader = new PropertyReader(ModelBuilder, property);
            }
            else if (memberSymbol is IEventSymbol @event)
            {
                memberReader = new EventReader(ModelBuilder, @event);
            }
            else
            {
                memberReader = null;
                return false;
            }

            return true;
        }

        private IPhasedMemberReader CreateMethodOrConstructorReader(IMethodSymbol methodSymbol)
        {
            return (
                methodSymbol.MethodKind == MethodKind.Constructor || methodSymbol.MethodKind == MethodKind.StaticConstructor  
                    ? new ConstructorReader(ModelBuilder, methodSymbol) as IPhasedMemberReader
                    : new MethodReader(ModelBuilder, methodSymbol) as IPhasedMemberReader);
        }

        private bool IsStandaloneMethodMember(IMethodSymbol method)
        {
            return (
                method.MethodKind == MethodKind.Ordinary ||
                method.MethodKind == MethodKind.Destructor ||
                method.MethodKind == MethodKind.StaticConstructor ||
                method.MethodKind == MethodKind.Constructor);
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