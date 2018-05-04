using MetaPrograms.CodeModel.Imperative.Expressions;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Statements
{
    public class ForEachStatement : AbstractStatement
    {
        public ForEachStatement(
            AbstractExpression enumerable, 
            BlockStatement body, 
            LocalVariable currentItem)
        {
            Enumerable = enumerable;
            Body = body;
            CurrentItem = currentItem;
        }

        public ForEachStatement(
            ForEachStatement source,
            Mutator<AbstractExpression>? enumerable = null,
            Mutator<BlockStatement>? body = null,
            Mutator<LocalVariable>? currentItem = null)
        {
            Enumerable = enumerable.MutatedOrOriginal(source.Enumerable);
            Body = body.MutatedOrOriginal(source.Body);
            CurrentItem = currentItem.MutatedOrOriginal(source.CurrentItem);
        }

        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitForEachStatement(this);

            if (Enumerable != null)
            {
                Enumerable.AcceptVisitor(visitor);
            }

            Body.AcceptVisitor(visitor);

            if (CurrentItem != null)
            {
                visitor.VisitReferenceToLocalVariable(CurrentItem);
            }
        }

        public AbstractExpression Enumerable { get; }
        public BlockStatement Body { get; }
        public LocalVariable CurrentItem { get; }
    }
}
