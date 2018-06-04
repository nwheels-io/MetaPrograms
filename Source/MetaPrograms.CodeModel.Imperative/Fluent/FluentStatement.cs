using MetaPrograms.CodeModel.Imperative.Expressions;
using MetaPrograms.CodeModel.Imperative.Statements;

// ReSharper disable InconsistentNaming

namespace MetaPrograms.CodeModel.Imperative.Fluent
{
    public class FluentStatement
    {
        public void RETURN(AbstractExpression value)
            => BlockContext.Append(new ReturnStatement(BlockContext.Pop(value)));

        public FluentIf IF(AbstractExpression condition)
            => new FluentIf(condition);
    }
}