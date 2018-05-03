using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Expressions
{
    public class IsExpression : AbstractExpression
    {
        public IsExpression(
            TypeMember type, 
            AbstractExpression expression, 
            LocalVariable patternMatchVariable) 
            : base(type)
        {
            Expression = expression;
            PatternMatchVariable = patternMatchVariable;
        }

        public IsExpression(
            IsExpression source,
            Mutator<TypeMember>? type = null,
            Mutator<AbstractExpression>? expression = null,
            Mutator<LocalVariable>? patternMatchVariable = null) 
            : base(source, type)
        {
            Expression = expression.MutatedOrOriginal(source.Expression);
            PatternMatchVariable = patternMatchVariable.MutatedOrOriginal(source.PatternMatchVariable);
        }

        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitIsExpression(this);

            if (Expression != null)
            {
                Expression.AcceptVisitor(visitor);
            }

            if (PatternMatchVariable != null)
            {
                visitor.VisitReferenceToLocalVariable(PatternMatchVariable);
            }
        }

        public AbstractExpression Expression { get; }
        public LocalVariable PatternMatchVariable { get; }
    }
}
