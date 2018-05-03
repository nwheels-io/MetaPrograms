using MetaPrograms.CodeModel.Imperative.Expressions;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Statements
{
    public class VariableDeclarationStatement : AbstractStatement
    {
        public VariableDeclarationStatement(
            LocalVariable variable, 
            AbstractExpression initialValue)
        {
            Variable = variable;
            InitialValue = initialValue;
        }

        public VariableDeclarationStatement(
            VariableDeclarationStatement source,
            Mutator<LocalVariable>? variable = null,
            Mutator<AbstractExpression>? initialValue = null)
        {
            Variable = variable.MutatedOrOriginal(source.Variable);
            InitialValue = initialValue.MutatedOrOriginal(source.InitialValue);
        }

        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitVariableDeclaraitonStatement(this);

            if (Variable != null)
            {
                visitor.VisitReferenceToLocalVariable(Variable);
            }

            if (InitialValue != null)
            {
                InitialValue.AcceptVisitor(visitor);
            }
        }

        public LocalVariable Variable { get; }
        public AbstractExpression InitialValue { get; }
    }
}
