using MetaPrograms.CodeModel.Imperative.Expressions;

namespace MetaPrograms.CodeModel.Imperative.Fluent
{
    public class FluentStatement
    {
        public void RETURN(AbstractExpression value) { }
        public FluentIf IF(AbstractExpression condition) => null;
    }
}