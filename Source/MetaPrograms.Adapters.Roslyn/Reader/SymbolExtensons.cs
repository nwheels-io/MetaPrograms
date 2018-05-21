using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MetaPrograms.CodeModel.Imperative.Members;
using Microsoft.CodeAnalysis;

namespace MetaPrograms.Adapters.Roslyn.Reader
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


        private static void AppendSystemTypeMetadataName(INamedTypeSymbol symbol, StringBuilder output)
        {
            if (symbol.TypeKind != TypeKind.Class && symbol.TypeKind != TypeKind.Struct && symbol.TypeKind != TypeKind.Interface && !symbol.IsGenericType)
            {
                output.Append(symbol.ToDisplayString(CSharpSymbolDisplayFormat));
                return;
            }

            if (symbol.ContainingSymbol is INamedTypeSymbol containingType &&
                (containingType.TypeKind == TypeKind.Class || containingType.TypeKind == TypeKind.Struct))
            {
                AppendSystemTypeMetadataName(containingType, output);
                output.Append('+');
            }
            else if (symbol.ContainingNamespace != null && !symbol.ContainingNamespace.IsGlobalNamespace)
            {
                output.Append(symbol.ContainingNamespace.ToDisplayString(CSharpSymbolDisplayFormat));
                output.Append('.');
            }

            output.Append(symbol.Name);

            if (symbol.IsGenericType)
            {
                output.Append('`');
                output.Append(symbol.Arity);

                if (symbol.TypeArguments != null)
                {
                    output.Append('[');
                    
                    for (int i = 0; i < symbol.TypeArguments.Length; i++)
                    {
                        if (i > 0)
                        {
                            output.Append(',');
                        }

                        output.Append('[');
                        
                        var parameterOrArgument = symbol.TypeArguments[i];
                        if (parameterOrArgument is INamedTypeSymbol argument)
                        {
                            AppendSystemTypeMetadataName(argument, output);
                        }
                        else
                        {
                            output.Append(parameterOrArgument.ToDisplayString(CSharpSymbolDisplayFormat));
                        }

                        output.Append(']');
                    }

                    output.Append(']');
                }
            }

            if (symbol.SpecialType == SpecialType.None && symbol.ContainingAssembly != null)
            {
                output.Append(',');
                output.Append(symbol.ContainingAssembly.Name);
            }
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
    }
}