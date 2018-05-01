using NWheels.CodeGeneration.CodeModel.Members;

namespace NWheels.CodeGeneration.CodeModel.Expressions
{
    public class IsExpression : AbstractExpression
    {
        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitIsExpression(this);

            if (Source != null)
            {
                Source.AcceptVisitor(visitor);
            }

            if (PatternMatchVariable != null)
            {
                visitor.VisitReferenceToLocalVariable(PatternMatchVariable);
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public AbstractExpression Source { get; set; }
        public LocalVariable PatternMatchVariable { get; set; }

    }
}
