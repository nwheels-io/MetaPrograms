namespace MetaPrograms.CodeModel.Imperative.Members
{
    public class LocalVariable
    {
        public LocalVariable(string name, TypeMember type)
        {
            Name = name;
            Type = type;
        }

        public string Name { get; }
        public TypeMember Type { get; }
    }
}
