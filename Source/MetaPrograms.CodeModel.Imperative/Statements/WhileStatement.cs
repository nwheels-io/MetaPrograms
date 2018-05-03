using MetaPrograms.CodeModel.Imperative.Expressions;

namespace MetaPrograms.CodeModel.Imperative.Statements
{
    public class WhileStatement : AbstractStatement
    {
        public WhileStatement(
            AbstractExpression condition, 
            BlockStatement body)
        {
            Condition = condition;
            Body = body;
        }

        public WhileStatement(
            WhileStatement source,
            Mutator<AbstractExpression>? condition = null,
            Mutator<BlockStatement>? body = null)
        {
            Condition = condition.MutatedOrOriginal(source.Condition);
            Body = body.MutatedOrOriginal(source.Body);
        }

        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitWhileStatement(this);
            
            if (Condition != null)
            {
                Condition.AcceptVisitor(visitor);
            }

            Body.AcceptVisitor(visitor);
        }

        public AbstractExpression Condition { get; }
        public BlockStatement Body { get; }
    }
}
