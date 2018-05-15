using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace MetaPrograms.Adapters.Roslyn.Reader
{
    public static class SymbolExtensons
    {
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

        public static bool IsGlobalNamespace(this ISymbol symbol)
        {
            return (symbol is INamespaceSymbol ns && ns.IsGlobalNamespace);
        }
    }
}