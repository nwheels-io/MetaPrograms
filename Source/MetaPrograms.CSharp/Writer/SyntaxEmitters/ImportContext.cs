using System;
using System.Collections.Generic;
using System.Text;
using MetaPrograms.Members;

namespace MetaPrograms.CSharp.Writer.SyntaxEmitters
{
    public class ImportContext
    {
        private readonly HashSet<TypeMember> _importedTypes = new HashSet<TypeMember>();
        private readonly HashSet<string> _importedNamespaces = new HashSet<string>();

        public void ImportType(TypeMember type)
        {
            _importedTypes.Add(type);
            _importedNamespaces.Add(type.Namespace);
        }

        public bool IsTypeImported(TypeMember type) => _importedTypes.Contains(type);

        public IEnumerable<TypeMember> ImportedTypes => _importedTypes;
        public IEnumerable<string> ImportedNamespaces => _importedNamespaces;
    }
}
