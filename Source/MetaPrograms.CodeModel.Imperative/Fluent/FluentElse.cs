using System;
using MetaPrograms.CodeModel.Imperative.Expressions;
using MetaPrograms.CodeModel.Imperative.Statements;

// ReSharper disable InconsistentNaming

namespace MetaPrograms.CodeModel.Imperative.Fluent
{
    public class FluentElse
    {
        private readonly IfStatement _statement;

        public FluentElse(IfStatement statement)
        {
            _statement = statement;
        }

        public FluentIf ELSEIF(AbstractExpression condition)
        {
            FluentIf result = null;

            ELSE(() => {
                result = new FluentIf(condition);
            });

            return result;
        }

        public void ELSE(Action body)
        {
            var container = new BlockContainer();

            using (CodeGeneratorContext.GetContextOrThrow().PushState(container))
            {
                body?.Invoke();
            }

            BlockContext.Replace(_statement, _statement.WithElseBlock(container.Block));
        }
    }
}