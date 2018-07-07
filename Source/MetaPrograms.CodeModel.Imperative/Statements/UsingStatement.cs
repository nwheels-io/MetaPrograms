using MetaPrograms.CodeModel.Imperative.Expressions;

namespace MetaPrograms.CodeModel.Imperative.Statements
{
    public class UsingStatement : AbstractStatement
    {
        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitUsingStatement(this);

            if (Disposable != null)
            {
                Disposable.AcceptVisitor(visitor);
            }

            Body.AcceptVisitor(visitor);
        }

        public AbstractExpression Disposable { get; set; }
        public BlockStatement Body { get; set; }
    }
}
