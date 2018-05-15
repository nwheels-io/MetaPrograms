namespace MetaPrograms.CodeModel.Imperative.Members
{
    public class LocalVariable
    {
        public LocalVariable(string name, MemberRef<TypeMember> type)
        {
            Name = name;
            Type = type;
        }

        public string Name { get; }
        public MemberRef<TypeMember> Type { get; }
    }
}
