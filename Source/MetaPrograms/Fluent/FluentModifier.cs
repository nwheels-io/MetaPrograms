using MetaPrograms.Members;

// ReSharper disable InconsistentNaming

namespace MetaPrograms.Fluent
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
        public FluentMember DEFAULT => new FluentMember(isDefaultExport: true);
    }
}
