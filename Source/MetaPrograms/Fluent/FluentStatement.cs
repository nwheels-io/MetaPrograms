using MetaPrograms.Expressions;
using MetaPrograms.Statements;

// ReSharper disable InconsistentNaming

namespace MetaPrograms.Fluent
{
    public class FluentStatement
    {
        public void RETURN(AbstractExpression value)
            => BlockContext.Append(new ReturnStatement {
                Expression = BlockContext.Pop(value)
            });

        public FluentIf IF(AbstractExpression condition)
            => new FluentIf(condition);
    }
}