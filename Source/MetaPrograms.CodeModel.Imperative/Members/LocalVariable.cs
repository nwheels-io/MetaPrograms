using MetaPrograms.CodeModel.Imperative.Expressions;

namespace MetaPrograms.CodeModel.Imperative.Members
{
    public class LocalVariable : IAssignable
    {
        public LocalVariable(string name, MemberRef<TypeMember> type)
        {
            Name = name;
            Type = type;
        }

        public string Name { get; }
        public MemberRef<TypeMember> Type { get; }

        public static implicit operator LocalVariableExpression(LocalVariable source)
        {
            return new LocalVariableExpression(source.Type, source);
        }
    }
}
