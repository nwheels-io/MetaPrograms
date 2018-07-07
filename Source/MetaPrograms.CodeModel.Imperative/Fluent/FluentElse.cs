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
            _statement.ElseBlock = new BlockStatement();

            using (CodeGeneratorContext.GetContextOrThrow().PushState(_statement.ElseBlock))
            {
                body?.Invoke();
            }
        }
    }
}