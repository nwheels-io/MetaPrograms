using System;

namespace MetaPrograms
{
    public class CodeReaderContext : CodeContextBase, IDisposable
    {
        public CodeReaderContext(ImperativeCodeModel codeModel, IClrTypeResolver clrTypeResolver, LanguageInfo language)
            : base(codeModel, clrTypeResolver, language)
        {
        }

        public override IdentifierName.OriginKind DefaultIdentifierOrigin => IdentifierName.OriginKind.Source;

        public static CodeReaderContext GetContextOrThrow() => GetContextOrThrow<CodeReaderContext>();
        public static CodeReaderContext CurrentContext => Current.Value as CodeReaderContext;
    }
}