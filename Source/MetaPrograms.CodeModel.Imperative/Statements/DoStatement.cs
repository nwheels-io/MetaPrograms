using MetaPrograms.CodeModel.Imperative.Expressions;

namespace MetaPrograms.CodeModel.Imperative.Statements
{
    public class DoStatement : AbstractStatement
    {
        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitDoStatement(this);

            Body.AcceptVisitor(visitor);

            if (Condition != null)
            {
                Condition.AcceptVisitor(visitor);
            }
        }

        public BlockStatement Body { get; set; }
        public AbstractExpression Condition { get; set; }
    }
}
