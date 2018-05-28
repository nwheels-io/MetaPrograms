using MetaPrograms.CodeModel.Imperative.Members;

// ReSharper disable InconsistentNaming

namespace MetaPrograms.CodeModel.Imperative.Fluent
{
    public class FluentVisibility : FluentModifier
    {
        public FluentVisibility(MemberVisibility visibility)
        {
            CodeGeneratorContext.GetContextOrThrow().PushState(new MemberTraitsContext(visibility));
        }

        public FluentModifier STATIC => new FluentModifier(MemberModifier.Static);
        public FluentModifier ABSTRACT => new FluentModifier(MemberModifier.Abstract);
        public FluentModifier VIRTUAL => new FluentModifier(MemberModifier.Virtual);
        public FluentModifier OVERRIDE => new FluentModifier(MemberModifier.Override);
    }
}