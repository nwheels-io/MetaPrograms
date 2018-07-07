using MetaPrograms.CodeModel.Imperative.Expressions;

namespace MetaPrograms.CodeModel.Imperative.Statements
{
    public class ExpressionStatement : AbstractStatement
    {
        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitExpressionStatement(this);

            if (Expression != null)
            {
                Expression.AcceptVisitor(visitor);
            }
        }

        public AbstractExpression Expression { get; set; }
    }
}
