using System;
using System.Collections.Generic;
using MetaPrograms.CodeModel.Imperative.Expressions;
using MetaPrograms.CodeModel.Imperative.Members;
using MetaPrograms.CodeModel.Imperative.Statements;

namespace MetaPrograms.CodeModel.Imperative.Fluent
{
    public class BlockContext : IDisposable
    {
        private readonly List<LocalVariable> _locals;
        private readonly List<AbstractStatement> _statements;
        private readonly HashSet<AbstractExpression> _expressionStack;

        public BlockContext(CodeGeneratorContext context)
            : this()
        {
            Container = context.PeekStateOrThrow<IBlockContainerContext>();
        }

        protected BlockContext()
        {
            _locals = new List<LocalVariable>();
            _statements = new List<AbstractStatement>();
            _expressionStack = new HashSet<AbstractExpression>();
        }

        public void AddLocal(LocalVariable local)
        {
            _locals.Add(local);
        }

        public T AppendStatement<T>(T statement)
            where T : AbstractStatement
        {
            ConvertStandaloneExpressionsToStatements();
            _statements.Add(statement);

            return statement;
        }

        public T ReplaceStatement<T>(T original, T replacement)
            where T : AbstractStatement
        {
            var index = _statements.IndexOf(original);
            if (index >= 0)
            {
                _statements[index] = replacement;
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
            Container.AttachBlock(_locals, _statements);
        }
        
        public IBlockContainerContext Container { get; protected set; }

        private void ConvertStandaloneExpressionsToStatements()
        {
            foreach (var expression in _expressionStack)
            {
                _statements.Add(new ExpressionStatement(expression));
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
