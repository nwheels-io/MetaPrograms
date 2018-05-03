using MetaPrograms.CodeModel.Imperative.Expressions;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Statements
{
    public class PropagateCallStatement : AbstractStatement
    {
        public PropagateCallStatement(AbstractExpression target, LocalVariable returnValue)
        {
            Target = target;
            ReturnValue = returnValue;
        }

        public PropagateCallStatement(
            PropagateCallStatement source,
            Mutator<AbstractExpression>? target = null,
            Mutator<LocalVariable>? returnValue = null)
        {
            Target = target.MutatedOrOriginal(source.Target);
            ReturnValue = returnValue.MutatedOrOriginal(source.ReturnValue);
        }

        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitPropagateCallStatement(this);

            if (Target != null)
            {
                Target.AcceptVisitor(visitor);
            }

            if (ReturnValue != null)
            {
                visitor.VisitReferenceToLocalVariable(ReturnValue);
            }
        }

        public AbstractExpression Target { get; }

        //TODO: what is this variable for?
        public LocalVariable ReturnValue { get; } 
    }
}
