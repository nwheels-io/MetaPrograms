using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommonExtensions;
using MetaPrograms.CSharp.Writer.SyntaxEmitters;
using MetaPrograms;
using MetaPrograms.Members;
using Microsoft.CodeAnalysis.CSharp;

namespace MetaPrograms.CSharp.Writer
{
    public class RoslynCodeModelWriter
    {
        private readonly ImperativeCodeModel _codeModel;
        private readonly ICodeGeneratorOutput _output;
        private readonly List<AbstractMember> _members = new List<AbstractMember>();

        public RoslynCodeModelWriter(ImperativeCodeModel codeModel, ICodeGeneratorOutput output)
        {
            _codeModel = codeModel;
            _output = output;
        }

        public void AddMembers(IEnumerable<AbstractMember> members)
        {
            _members.AddRange(members);
        }

        public void WriteAll()
        {
            var commonNamespaceParts = FindCommonNamespace();

            foreach (var type in _members.OfType<TypeMember>().Where(t => t.DeclaringType == null))
            {
                WriteType(type, commonNamespaceParts);
            }
        }

        private IList<string> FindCommonNamespace()
        {
            return _members
                .OfType<TypeMember>()
                .Where(t => t.Namespace != null)
                .Select(m => m.Namespace.Split('.'))
                .FindCommonPrefix();
        }

        private void WriteType(TypeMember type, IList<string> commonNamespaceParts)
        {
            var syntaxBuilder = new UnitSyntaxEmitter(_codeModel, type);
            var syntaxTree = syntaxBuilder.EmitSyntax();

            var subFolderParts = GetSubFolder(type, commonNamespaceParts);
            _output.AddSourceFile(subFolderParts, $"{type.Name}.cs", syntaxTree.ToString());
        }

        private static IEnumerable<string> GetSubFolder(TypeMember type, IList<string> commonNamespaceParts)
        {
            var namespaceParts = type.Namespace?.Split('.') ?? new string[0];
            return namespaceParts.Skip(commonNamespaceParts.Count);
        }

        private IReadOnlyCollection<TypeMember> GetAllReferencedTypes(IEnumerable<TypeMember> typesToCompile)
        {
            var referencedTypes = new HashSet<TypeMember>();
            var visitor = new TypeReferenceMemberVisitor(referencedTypes);

            foreach (var type in typesToCompile)
            {
                type.AcceptVisitor(visitor);
            }

            return referencedTypes;
        }
    }
}
