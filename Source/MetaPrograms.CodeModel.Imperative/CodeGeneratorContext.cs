using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using MetaPrograms.CodeModel.Imperative.Expressions;
using MetaPrograms.CodeModel.Imperative.Fluent;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative
{
    public class CodeGeneratorContext : CodeContextBase
    {
        private readonly List<AbstractMember> _generatedMembers = new List<AbstractMember>();

        public CodeGeneratorContext(ImperativeCodeModel codeModel, IClrTypeResolver clrTypeResolver, LanguageInfo language)
            : base(codeModel, clrTypeResolver, language)
        {
        }

        public void AddGeneratedMember<TMember>(TMember member, bool isTopLevel)
            where TMember : AbstractMember
        {
            _generatedMembers.Add(member);
            this.CodeModel.Add(member, isTopLevel);
        }

        public override IdentifierName.OriginKind DefaultIdentifierOrigin => IdentifierName.OriginKind.Generator;
        public IEnumerable<AbstractMember> GeneratedMembers => _generatedMembers;

        public static CodeGeneratorContext GetContextOrThrow() => GetContextOrThrow<CodeGeneratorContext>();
        public static CodeGeneratorContext CurrentContext => Current.Value as CodeGeneratorContext;
    }
}
