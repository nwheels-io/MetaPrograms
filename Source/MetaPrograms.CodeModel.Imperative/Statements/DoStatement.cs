using MetaPrograms.CodeModel.Imperative.Expressions;

namespace MetaPrograms.CodeModel.Imperative.Statements
{
    public class DoStatement : AbstractStatement
    {
        public DoStatement(BlockStatement body, AbstractExpression condition)
        {
            Body = body;
            Condition = condition;
        }

        public DoStatement(
            DoStatement source,
            Mutator<BlockStatement>? body = null,
            Mutator<AbstractExpression>? condition = null)
        {
            Body = body.MutatedOrOriginal(source.Body);
            Condition = condition.MutatedOrOriginal(source.Condition);
        }

        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitDoStatement(this);

            Body.AcceptVisitor(visitor);

            if (Condition != null)
            {
                Condition.AcceptVisitor(visitor);
            }
        }

        public BlockStatement Body { get; }
        public AbstractExpression Condition { get; }
    }
}
