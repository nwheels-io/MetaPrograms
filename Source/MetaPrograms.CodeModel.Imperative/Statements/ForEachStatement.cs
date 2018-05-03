using MetaPrograms.CodeModel.Imperative.Expressions;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Statements
{
    public class ForEachStatement : AbstractStatement
    {
        public ForEachStatement()
        {
            this.Body = new BlockStatement();
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

        public AbstractExpression Enumerable { get; set; }
        public BlockStatement Body { get; }
        public LocalVariable CurrentItem { get; set; }
    }
}
