using MetaPrograms.Members;

namespace MetaPrograms.Fluent
{
    public class MemberTraitsContext
    {
        public MemberTraitsContext(MemberVisibility visibility)
        {
            Visibility = visibility;
            Modifier = MemberModifier.None;
        }

        public MemberVisibility Visibility { get; }
        public MemberModifier Modifier { get; set; }
        public bool IsAsync { get; set; }
        public bool IsReadOnly { get; set; }
        public bool IsDefaultExport { get; set; }
    }
}
