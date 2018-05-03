using MetaPrograms.CodeModel.Imperative.Expressions;

namespace MetaPrograms.CodeModel.Imperative.Statements
{
    public class IfStatement : AbstractStatement
    {
        public IfStatement(AbstractExpression condition, BlockStatement thenBlock, BlockStatement elseBlock)
        {
            Condition = condition;
            ThenBlock = thenBlock;
            ElseBlock = elseBlock;
        }

        public IfStatement(
            IfStatement source, 
            Mutator<AbstractExpression>? condition = null,
            Mutator<BlockStatement>? thenBlock = null,
            Mutator<BlockStatement>? elseBlock = null)
        {
            Condition = condition.MutatedOrOriginal(source.Condition);
            ThenBlock = thenBlock.MutatedOrOriginal(source.ThenBlock);
            ElseBlock = elseBlock.MutatedOrOriginal(source.ElseBlock);
        }

        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitIfStatement(this);

            if (Condition != null)
            {
                Condition.AcceptVisitor(visitor);
            }

            if (ThenBlock != null)
            {
                ThenBlock.AcceptVisitor(visitor);
            }

            if (ElseBlock != null)
            {
                ElseBlock.AcceptVisitor(visitor);
            }
        }

        public AbstractExpression Condition { get; }
        public BlockStatement ThenBlock { get; }
        public BlockStatement ElseBlock { get; }
    }
}
