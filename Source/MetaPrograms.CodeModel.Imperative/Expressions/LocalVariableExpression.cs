using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Expressions
{
    public class LocalVariableExpression : AbstractExpression, IAssignable
    {
        public LocalVariableExpression(
            MemberRef<TypeMember> type, 
            LocalVariable variable) 
            : base(type)
        {
            Variable = variable;
        }

        public LocalVariableExpression(
            LocalVariableExpression source,
            Mutator<LocalVariable>? variable = null, 
            Mutator<MemberRef<TypeMember>>? type = null) 
            : base(source, type)
        {
            Variable = variable.MutatedOrOriginal(source.Variable);
        }

        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitLocalVariableExpression(this);

            if (Variable != null)
            {
                visitor.VisitReferenceToLocalVariable(Variable);
            }
        }

        public LocalVariable Variable { get; }
    }
}
