using MetaPrograms.CodeModel.Imperative.Members;

// ReSharper disable InconsistentNaming

namespace MetaPrograms.CodeModel.Imperative.Fluent
{
    public class FluentModifier : FluentMember
    {
        public FluentModifier(MemberModifier modifier)
        {
            CodeGeneratorContext.GetContextOrThrow().PeekStateOrThrow<MemberTraitsContext>().Modifier = modifier;
        }

        protected FluentModifier()
        {
        }

        public FluentMember ASYNC => new FluentMember(isAsync: true);
        public FluentMember READONLY => new FluentMember(isReadOnly: true);
    }
}
