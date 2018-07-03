using MetaPrograms.CodeModel.Imperative.Expressions;

namespace MetaPrograms.CodeModel.Imperative.Members
{
    public class LocalVariable : IAssignable
    {
        public LocalVariable(string name, MemberRef<TypeMember> type, bool isFinal = false)
        {
            Name = name;
            Type = type;
            IsFinal = isFinal;
        }

        public string Name { get; }
        public MemberRef<TypeMember> Type { get; }
        public bool IsFinal { get; }

        public static implicit operator LocalVariableExpression(LocalVariable source)
        {
            return new LocalVariableExpression(source.Type, source);
        }

        public AbstractExpression AsExpression()
        {
            return new LocalVariableExpression(this.Type, this);
        }

        public void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitReferenceToLocalVariable(this);
        }
    }
}
