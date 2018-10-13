using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MetaPrograms.Members;
using Microsoft.CodeAnalysis;

namespace MetaPrograms.CSharp.Reader
{
    public static class SymbolExtensons
    {
        public static readonly SymbolDisplayFormat CSharpSymbolDisplayFormat = new SymbolDisplayFormat(
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
            genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters);
        
        public static string GetFullNamespaceName(this ISymbol symbol)
        {
            var namespaceHierarchy = new List<string>();
            
            for (
                var namespaceSymbol = symbol.ContainingNamespace;
                namespaceSymbol != null && !namespaceSymbol.IsGlobalNamespace;
                namespaceSymbol = namespaceSymbol.ContainingNamespace)
            {
                namespaceHierarchy.Add(namespaceSymbol.Name);
            }

            return string.Join(".", Enumerable.Reverse(namespaceHierarchy));
        }

        public static string GetFullyQualifiedMetadataName(this INamespaceOrTypeSymbol symbol)
        {
            var result = new StringBuilder(symbol.MetadataName, capacity: 255);
            
            for (
                var outerSymbol = symbol.ContainingSymbol;
                outerSymbol != null && !outerSymbol.IsGlobalNamespace();
                outerSymbol = outerSymbol.ContainingSymbol)
            {
                result.Insert(0, outerSymbol is INamespaceSymbol ? '.' : '+');
                result.Insert(0, outerSymbol.OriginalDefinition.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat));
            }

            return result.ToString();
        }

        public static string GetSystemTypeMetadataName(this INamedTypeSymbol symbol)
        {
            var result = new StringBuilder(capacity: 255);
            AppendSystemTypeMetadataName(symbol, result);            
            return result.ToString();
        }
        
        public static bool IsGlobalNamespace(this ISymbol symbol)
        {
            return (symbol is INamespaceSymbol ns && ns.IsGlobalNamespace);
        }

        public static MemberVisibility GetMemberVisibility(this ISymbol symbol)
        {
            switch (symbol.DeclaredAccessibility)
            {
                case Accessibility.Private: 
                    return MemberVisibility.Private;
                case Accessibility.ProtectedAndInternal: 
                    return MemberVisibility.PrivateProtected;
                case Accessibility.Protected: 
                    return MemberVisibility.Protected;
                case Accessibility.Internal: 
                    return MemberVisibility.Internal;
                case Accessibility.ProtectedOrInternal: 
                    return MemberVisibility.InternalProtected;
                case Accessibility.Public: 
                    return MemberVisibility.Public;
                default: 
                    throw new ArgumentOutOfRangeException($"Accessibility value not supported: {symbol.DeclaredAccessibility}");
            }
        }

        public static MemberModifier GetMemberModifier(this ISymbol symbol)
        {
            if (symbol.IsStatic)
            {
                return MemberModifier.Static;
            }
            else if (symbol.IsAbstract)
            {
                return MemberModifier.Abstract;
            }
            else if (symbol.IsOverride)
            {
                return MemberModifier.Override;
            }
            else if (symbol.IsVirtual)
            {
                return MemberModifier.Virtual;
            }

            return MemberModifier.None;
        }

        public static MethodParameterModifier GetParameterModifier(this IParameterSymbol symbol)
        {
            switch (symbol.RefKind)
            {
                case RefKind.Ref:
                    return MethodParameterModifier.Ref;
                case RefKind.RefReadOnly:
                    return MethodParameterModifier.RefReadonly;
                case RefKind.Out:
                    return MethodParameterModifier.Out;
                default:
                    return MethodParameterModifier.None;
            }
        }

        public static MethodParameterModifier GetReturnValueModifier(this IMethodSymbol symbol)
        {
            if (symbol.ReturnsByRefReadonly)
            {
                return MethodParameterModifier.RefReadonly;
            } 
            else if (symbol.ReturnsByRef)
            {
                return MethodParameterModifier.Ref;
            }

            return MethodParameterModifier.None;
        }
        
        private static void AppendSystemTypeMetadataName(INamedTypeSymbol symbol, StringBuilder output, bool isContainingType = false)
        {
            if (!ShouldApplySystemTypeNameLogic(symbol))
            {
                output.Append(symbol.ToDisplayString(CSharpSymbolDisplayFormat));
                return;
            }

            AppendSystemTypeNameQualifiers(symbol, output);

            output.Append(symbol.Name);

            if (symbol.IsGenericType)
            {
                AppendSystemTypeNameGenerics(symbol, output);
            }

            if (!isContainingType)
            {
                AppendSystemTypeNameAssembly(symbol, output);
            }
        }

        private static bool ShouldApplySystemTypeNameLogic(INamedTypeSymbol symbol)
        {
            return (
                symbol.TypeKind == TypeKind.Class || 
                symbol.TypeKind == TypeKind.Struct || 
                symbol.TypeKind == TypeKind.Interface || 
                symbol.IsGenericType);
        }

        private static void AppendSystemTypeNameQualifiers(INamedTypeSymbol symbol, StringBuilder output)
        {
            if (symbol.ContainingSymbol is INamedTypeSymbol containingType &&
                (containingType.TypeKind == TypeKind.Class || containingType.TypeKind == TypeKind.Struct))
            {
                AppendSystemTypeMetadataName(containingType, output, isContainingType: true);
                output.Append('+');
            }
            else if (symbol.ContainingNamespace != null && !symbol.ContainingNamespace.IsGlobalNamespace)
            {
                output.Append(symbol.ContainingNamespace.ToDisplayString(CSharpSymbolDisplayFormat));
                output.Append('.');
            }
        }

        private static void AppendSystemTypeNameGenerics(INamedTypeSymbol symbol, StringBuilder output)
        {
            output.Append('`');
            output.Append(symbol.Arity);

            if (symbol.TypeArguments != null && symbol.TypeArguments.All(t => t is INamedTypeSymbol))
            {
                output.Append('[');

                for (int i = 0; i < symbol.TypeArguments.Length; i++)
                {
                    if (i > 0)
                    {
                        output.Append(',');
                    }

                    output.Append('[');
                    AppendSystemTypeMetadataName((INamedTypeSymbol)symbol.TypeArguments[i], output);
                    output.Append(']');
                }

                output.Append(']');
            }
        }

        private static void AppendSystemTypeNameAssembly(INamedTypeSymbol symbol, StringBuilder output)
        {
            if (symbol.SpecialType == SpecialType.None && symbol.ContainingAssembly != null)
            {
                output.Append(',');
                output.Append(symbol.ContainingAssembly.Name);
            }
        }
    }
}