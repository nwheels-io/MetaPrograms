using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MetaPrograms;
using MetaPrograms.Expressions;
using MetaPrograms.Members;
using MetaPrograms.Statements;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MetaPrograms.CSharp.Writer.SyntaxEmitters
{
    public static class SyntaxHelpers
    {
        private static readonly IReadOnlyDictionary<Type, SyntaxKind> _s_keywordPerType = new Dictionary<Type, SyntaxKind> {
            [typeof(bool)] = SyntaxKind.BoolKeyword,
            [typeof(byte)] = SyntaxKind.ByteKeyword,
            [typeof(sbyte)] = SyntaxKind.SByteKeyword,
            [typeof(short)] = SyntaxKind.ShortKeyword,
            [typeof(ushort)] = SyntaxKind.UShortKeyword,
            [typeof(int)] = SyntaxKind.IntKeyword,
            [typeof(uint)] = SyntaxKind.UIntKeyword,
            [typeof(long)] = SyntaxKind.LongKeyword,
            [typeof(ulong)] = SyntaxKind.ULongKeyword,
            [typeof(double)] = SyntaxKind.DoubleKeyword,
            [typeof(float)] = SyntaxKind.FloatKeyword,
            [typeof(decimal)] = SyntaxKind.DecimalKeyword,
            [typeof(string)] = SyntaxKind.StringKeyword,
            [typeof(char)] = SyntaxKind.CharKeyword,
            [typeof(object)] = SyntaxKind.ObjectKeyword,
        };

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public static AttributeSyntax ToAttributeSyntax(this AttributeDescription description)
        {
            return AttributeSyntaxEmitter.EmitSyntax(description);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public static ExpressionSyntax GetLiteralSyntax(object value)
        {
            if (value is ConstantExpression expression)
            {
                value = expression.Value;
            }
            if (value == null)
            {
                return SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);
            }
            else if (value is int intValue)
            {
                return SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(intValue));
            }
            else if (value is float floatValue)
            {
                return SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(floatValue));
            }
            else if (value is long longValue)
            {
                return SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(longValue));
            }
            else if (value is decimal decimalValue)
            {
                return SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(decimalValue));
            }
            else if (value is uint uintValue)
            {
                return SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(uintValue));
            }
            else if (value is double doubleValue)
            {
                return SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(doubleValue));
            }
            else if (value is ulong ulongValue)
            {
                return SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(ulongValue));
            }
            else if (value is string stringValue)
            {
                return SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(stringValue));
            }
            else if (value is char charValue)
            {
                return SyntaxFactory.LiteralExpression(SyntaxKind.CharacterLiteralExpression, SyntaxFactory.Literal(charValue));
            }
            else if (value is bool boolValue)
            {
                return SyntaxFactory.LiteralExpression(boolValue ? SyntaxKind.TrueLiteralExpression : SyntaxKind.FalseLiteralExpression);
            }
            else if (value is Type typeValue)
            {
                var context = CodeGeneratorContext.GetContextOrThrow();
                return SyntaxFactory.TypeOfExpression(GetTypeNameSyntax(context.FindType(typeValue)));
            }
            else if (value is TypeMember typeMember)
            {
                return SyntaxFactory.TypeOfExpression(GetTypeNameSyntax(typeMember));
            }

            throw new NotSupportedException($"Literals of type {value.GetType().Name} are not supported");
        }

        public static TypeSyntax GetTypeNameSyntax(this TypeMember type)
        {
            var clrBinding = type.Bindings.FirstOrDefault<System.Type>();
            
            if (clrBinding != null)
            {
                if (clrBinding.IsArray)
                {
                    var elementTypeSyntax = GetTypeNameSyntax(type.UnderlyingType);
                    return SyntaxFactory.ArrayType(elementTypeSyntax)
                        .WithRankSpecifiers(SyntaxFactory.SingletonList(SyntaxFactory.ArrayRankSpecifier(SyntaxFactory.SingletonSeparatedList<ExpressionSyntax>(SyntaxFactory.OmittedArraySizeExpression()))));
                }

                if (_s_keywordPerType.TryGetValue(clrBinding, out SyntaxKind keyword))
                {
                    return SyntaxFactory.PredefinedType(SyntaxFactory.Token(keyword));
                }

                if (IsNullableValueType(clrBinding.GetTypeInfo(), out Type underlyingValueType))
                {
                    var context = CodeGeneratorContext.GetContextOrThrow();
                    return SyntaxFactory.NullableType(GetTypeNameSyntax(context.FindType(underlyingValueType)));
                }
            }

            return GetTypeFullNameSyntax(type);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public static NameSyntax GetTypeFullNameSyntax(this TypeMember type, string stripSuffix = null)
        {
            var nonQuialifiedName = (string.IsNullOrEmpty(stripSuffix) ? type.Name : type.Name.TrimSuffixFragment(stripSuffix));

            if (!type.IsGenericType)
            {
                return QualifyTypeNameSyntax(type, SyntaxFactory.IdentifierName(nonQuialifiedName.ToPascalCase()));
            }

            var genericSyntax = SyntaxFactory.GenericName(type.Name)
                .WithTypeArgumentList(
                    SyntaxFactory.TypeArgumentList(
                        SyntaxFactory.SeparatedList<TypeSyntax>(
                            type.GenericArguments.Select(GetTypeNameSyntax))));

            return QualifyTypeNameSyntax(type, genericSyntax);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public static ArgumentListSyntax GetArgumentListSyntax(this MethodCallExpression call)
        {
            IEnumerable<ArgumentSyntax> argumentSyntaxes = call.Arguments.Select(EmitArgument);
            return SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(argumentSyntaxes));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public static BlockSyntax ToSyntax(this BlockStatement block)
        {
            if (block != null)
            {
                return SyntaxFactory.Block(block.Statements.Select(StatementSyntaxEmitter.EmitSyntax));
            }

            return SyntaxFactory.Block();
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public static string GetValidCSharpIdentifier(string proposedName)
        {
            if (string.IsNullOrEmpty(proposedName) || SyntaxFacts.IsValidIdentifier(proposedName))
            {
                return proposedName;
            }

            var validatedName = new char[proposedName.Length];

            for (int i = 0 ; i < proposedName.Length ; i++)
            {
                var c = proposedName[i];

                if (!char.IsLetter(c) && !(char.IsDigit(c) && i > 0) && !(c == '_'))
                {
                    validatedName[i] = '_';
                }
                else
                {
                    validatedName[i] = c;
                }
            }

            return new string(validatedName);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private static NameSyntax QualifyTypeNameSyntax(TypeMember type, SimpleNameSyntax simpleName)
        {
            if (type.DeclaringType != null)
            {
                return SyntaxFactory.QualifiedName(GetTypeFullNameSyntax(type.DeclaringType), simpleName);
            }

            if (!string.IsNullOrEmpty(type.Namespace))
            {
                var isNamespaceImported = IsTypeNamespaceImported(type);

                if (!isNamespaceImported)
                {
                    return SyntaxFactory.QualifiedName(SyntaxFactory.ParseName(type.Namespace), simpleName);
                }
            }

            return simpleName;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private static bool IsTypeNamespaceImported(TypeMember type)
        {
            var importContext = CodeGeneratorContext.GetContextOrThrow().LookupStateOrThrow<ImportContext>();

            if (importContext.IsTypeImported(type))
            {
                return true;
            }

            if (type.IsGenericType && type.GenericTypeDefinition != null)
            {
                return importContext.IsTypeImported(type.GenericTypeDefinition);
            }

            return false;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private static bool IsNullableValueType(System.Reflection.TypeInfo typeInfo, out Type underlyingValueType)
        {
            if (typeInfo.IsGenericType && 
                !typeInfo.IsGenericTypeDefinition && 
                typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                underlyingValueType = typeInfo.GetGenericArguments()[0];
                return true;
            }

            underlyingValueType = null;
            return false;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private static ArgumentSyntax EmitArgument(Argument argument)
        {
            var syntax = SyntaxFactory.Argument(ExpressionSyntaxEmitter.EmitSyntax(argument.Expression));

            switch (argument.Modifier)
            {
                case MethodParameterModifier.Ref:
                    syntax = syntax.WithRefOrOutKeyword(SyntaxFactory.Token(SyntaxKind.RefKeyword));
                    break;
                case MethodParameterModifier.Out:
                    syntax = syntax.WithRefOrOutKeyword(SyntaxFactory.Token(SyntaxKind.OutKeyword));
                    break;
            }

            return syntax;
        }
    }
}
