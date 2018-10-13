using System;
using MetaPrograms.Members;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MetaPrograms.CSharp.Writer.SyntaxEmitters
{
    public class TypeSyntaxEmitter
    {
        public static MemberDeclarationSyntax GetSyntax(TypeMember type)
        {
            var emitter = GetSyntaxEmitter(type);
            var typeSyntax = (MemberDeclarationSyntax)emitter.EmitSyntax();

            //TODO: do we need these annotations?
            //var annotation = new SyntaxAnnotation();
            //typeSyntax = typeSyntax.WithAdditionalAnnotations(annotation);
            //type.SafeBackendTag().Annotation = annotation;

            return typeSyntax;
        }

        public static ISyntaxEmitter GetSyntaxEmitter(TypeMember type)
        {
            switch (type.TypeKind)
            {
                //TODO: add support for all type kinds
                case TypeMemberKind.Class:
                    return new ClassSyntaxEmitter(type);
                case TypeMemberKind.Enum:
                    return new EnumSyntaxEmitter(type);
                default:
                    throw new NotSupportedException($"TypeMember of kind '{type.TypeKind}' is not supported.");
            }
        }
    }
}