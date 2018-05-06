using System.Collections.Generic;
using System.Linq;
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
                namespaceSymbol != null;
                namespaceSymbol = namespaceSymbol.ContainingNamespace)
            {
                namespaceHierarchy.Add(namespaceSymbol.Name);
            }

            return string.Join(".", Enumerable.Reverse(namespaceHierarchy));
        }
    }
}