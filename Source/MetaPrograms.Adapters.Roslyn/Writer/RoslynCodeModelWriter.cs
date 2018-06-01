using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommonExtensions;
using MetaPrograms.CodeModel.Imperative;
using MetaPrograms.CodeModel.Imperative.Members;
using Microsoft.CodeAnalysis.CSharp;

namespace MetaPrograms.Adapters.Roslyn.Writer
{
    public class RoslynCodeModelWriter
    {
        private readonly ImperativeCodeModel _codeModel;
        private readonly ICodeGeneratorOutput _output;
        private readonly List<IMemberRef> _members = new List<IMemberRef>();

        public RoslynCodeModelWriter(ImperativeCodeModel codeModel, ICodeGeneratorOutput output)
        {
            _codeModel = codeModel;
            _output = output;
        }

        public void AddMembers(IEnumerable<IMemberRef> members)
        {
            _members.AddRange(members);
        }

        public void WriteAll()
        {
            var commonNamespaceParts = FindCommonNamespace();

            foreach (var type in _members.Select(m => m.Get()).OfType<TypeMember>())
            {
                WriteType(type, commonNamespaceParts);
            }
        }

        private IList<string> FindCommonNamespace()
        {
            return _members
                .Select(m => m.Get())
                .OfType<TypeMember>()
                .Where(t => t.Namespace != null)
                .Select(m => m.Namespace.Split('.'))
                .FindCommonPrefix();
        }

        private void WriteType(TypeMember type, IList<string> commonNamespaceParts)
        {
            var syntaxBuilder = new FileSyntaxBuilder(_codeModel, type);
            var syntaxTree = syntaxBuilder.BuildSyntaxTree();

            var subFolderParts = GetSubFolder(type, commonNamespaceParts);
            _output.AddSourceFile(subFolderParts, $"{type.Name}.cs", syntaxTree.ToString());
        }

        private static IEnumerable<string> GetSubFolder(TypeMember type, IList<string> commonNamespaceParts)
        {
            var namespaceParts = type.Namespace?.Split('.') ?? new string[0];
            return namespaceParts.Skip(commonNamespaceParts.Count);
        }
    }
}
