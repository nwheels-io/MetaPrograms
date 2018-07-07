using MetaPrograms.CodeModel.Imperative.Expressions;

namespace MetaPrograms.CodeModel.Imperative.Statements
{
    public class WhileStatement : AbstractStatement
    {
        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitWhileStatement(this);
            
            if (Condition != null)
            {
                Condition.AcceptVisitor(visitor);
            }

            Body.AcceptVisitor(visitor);
        }

        public AbstractExpression Condition { get; set; }
        public BlockStatement Body { get; set; }
    }
}
