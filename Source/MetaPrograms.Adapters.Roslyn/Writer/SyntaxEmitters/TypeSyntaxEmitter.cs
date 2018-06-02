using System;
using MetaPrograms.CodeModel.Imperative.Members;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NWheels.Compilation.Adapters.Roslyn.SyntaxEmitters;

namespace MetaPrograms.Adapters.Roslyn.Writer.SyntaxEmitters
{
    public class TypeSyntaxEmitter
    {
        public static MemberDeclarationSyntax GetSyntax(TypeMember type)
        {
            MemberDeclarationSyntax typeSyntax;

            switch (type.TypeKind)
            {
                //TODO: add support for all type kinds
                case TypeMemberKind.Class:
                    var classEmitter = new ClassSyntaxEmitter(type);
                    typeSyntax = classEmitter.EmitSyntax();
                    break;
                case TypeMemberKind.Enum:
                    var enumEmitter = new EnumSyntaxEmitter(type);
                    typeSyntax = enumEmitter.EmitSyntax();
                    break;
                default:
                    throw new NotSupportedException($"TypeMember of kind '{type.TypeKind}' is not supported.");
            }

            //TODO: do we need these annotations?
            //var annotation = new SyntaxAnnotation();
            //typeSyntax = typeSyntax.WithAdditionalAnnotations(annotation);
            //type.SafeBackendTag().Annotation = annotation;

            return typeSyntax;
        }
    }
}