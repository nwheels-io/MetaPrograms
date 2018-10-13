//using System;
//using MetaPrograms;
//using MetaPrograms.Members;
//using Microsoft.CodeAnalysis;

//namespace MetaPrograms.CSharp.Reader
//{
//    public static class CodeModelBuilderExtensions
//    {
//        public static TypeMember IncludeType(this CodeModelBuilder modelBuilder, ITypeSymbol symbol)
//        {
//            return modelBuilder.GetOrAddMember(symbol, () => ReadType(modelBuilder, symbol));
//        }

//        public static TypeMember ReadType(this CodeModelBuilder modelBuilder, ITypeSymbol symbol)
//        {
//            switch (symbol.TypeKind)
//            {
//                case TypeKind.Class:
//                    return new ClassReader(CreateReaderMechanism()).Read();
//                case TypeKind.Interface:
//                    return new InterfaceReader(CreateReaderMechanism()).Read();
//                //case TypeKind.TypeParameter:
//                //    return new TypeParameterReader(modelBuilder, (ITypeParameterSymbol)symbol).Read();
//                default:
//                    throw new NotImplementedException(symbol.TypeKind.ToString());
//            }

//            TypeReaderMechanism CreateReaderMechanism()
//            {
//                return new TypeReaderMechanism(modelBuilder, (INamedTypeSymbol)symbol);
//            }
//        }
//    }
//}