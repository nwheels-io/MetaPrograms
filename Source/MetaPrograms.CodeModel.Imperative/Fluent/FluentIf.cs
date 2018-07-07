using System;
using MetaPrograms.CodeModel.Imperative.Expressions;
using MetaPrograms.CodeModel.Imperative.Statements;

// ReSharper disable InconsistentNaming

namespace MetaPrograms.CodeModel.Imperative.Fluent
{
    public class FluentIf
    {
        private readonly IfStatement _statement;

        public FluentIf(AbstractExpression condition)
        {
            var block = BlockContext.GetBlockOrThrow();

            _statement = new IfStatement {
                Condition = block.PopExpression(condition),
                ThenBlock = null,
                ElseBlock = null
            };

            block.AppendStatement(_statement);
        }

        public FluentElse THEN(Action body)
        {
            _statement.ThenBlock = new BlockStatement();

            var context = CodeGeneratorContext.GetContextOrThrow(); 
            using (context.PushState(new BlockContext(_statement.ThenBlock)))
            {
                body?.Invoke();
            }

            return new FluentElse(_statement);
        }
    }
}