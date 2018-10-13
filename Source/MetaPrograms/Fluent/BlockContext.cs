using System;
using System.Collections.Generic;
using MetaPrograms.Expressions;
using MetaPrograms.Members;
using MetaPrograms.Statements;

namespace MetaPrograms.Fluent
{
    public class BlockContext : IDisposable
    {
        private readonly HashSet<AbstractExpression> _expressionStack;

        public BlockContext()
            : this(new BlockStatement())
        {
        }

        public BlockContext(BlockStatement block)
        {
            _expressionStack = new HashSet<AbstractExpression>();
            this.Block = block;
        }

        public void AddLocal(LocalVariable local)
        {
            Block.Locals.Add(local);
        }

        public T AppendStatement<T>(T statement)
            where T : AbstractStatement
        {
            ConvertStandaloneExpressionsToStatements();
            Block.Statements.Add(statement);

            return statement;
        }

        public T ReplaceStatement<T>(T original, T replacement)
            where T : AbstractStatement
        {
            var index = Block.Statements.IndexOf(original);
            if (index >= 0)
            {
                Block.Statements[index] = replacement;
            }

            return replacement;
        }

        public T PushExpression<T>(T expression)
            where T : AbstractExpression
        {
            if (expression != null)
            {
                _expressionStack.Add(expression);
            }

            return expression;
        }

        public T PopExpression<T>(T expression)
            where T : AbstractExpression
        {
            if (expression != null)
            {
                _expressionStack.Remove(expression);
            }

            return expression;
        }

        public void Dispose()
        {
            ConvertStandaloneExpressionsToStatements();
        }

        public BlockStatement Block { get; }

        private void ConvertStandaloneExpressionsToStatements()
        {
            foreach (var expression in _expressionStack)
            {
                Block.Statements.Add(new ExpressionStatement {
                    Expression = expression
                });
            }

            _expressionStack.Clear();
        }

        public static BlockContext GetBlockOrThrow()
        {
            return CodeGeneratorContext.GetContextOrThrow().GetCurrentBlock();
        }

        public static T Append<T>(T statement)
            where T : AbstractStatement
        {
            return GetBlockOrThrow().AppendStatement(statement);
        }

        public static T Replace<T>(T original, T replacement)
            where T : AbstractStatement
        {
            return GetBlockOrThrow().ReplaceStatement(original, replacement);
        }

        public static T Push<T>(T expression)
            where T : AbstractExpression
        {
            return GetBlockOrThrow().PushExpression<T>(expression);
        }

        public static T Pop<T>(T expression)
            where T : AbstractExpression
        {
            return GetBlockOrThrow().PopExpression<T>(expression);
        }
    }
}
