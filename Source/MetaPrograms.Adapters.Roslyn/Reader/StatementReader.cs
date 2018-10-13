#if false

//TO BE REPLACED BY OperationReader

using System;
using System.Collections.Generic;
using MetaPrograms;
using MetaPrograms.Expressions;
using MetaPrograms.Statements;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MetaPrograms.Adapters.Roslyn.Reader
{
    public static class StatementReader
    {
        private static readonly Dictionary<Type, Func<CodeModelBuilder, StatementSyntax, AbstractStatement>> ReaderBySyntaxType =
            new Dictionary<Type, Func<CodeModelBuilder, StatementSyntax, AbstractStatement>> {
                [typeof(ExpressionStatementSyntax)] = (m, s) => ReadExpression(m, (ExpressionStatementSyntax)s)
            };

        public static AbstractStatement ReadStatement(CodeModelBuilder model, StatementSyntax statement)
        {
            if (ReaderBySyntaxType.TryGetValue(statement.GetType(), out var reader))
            {
                return reader(model, statement);
            }
            else
            {
                throw new NotSupportedException(
                    $"Statement of type '{statement.GetType().Name}' is not supported by {nameof(StatementReader)}.");
            }
        }

        private static AbstractStatement ReadExpression(CodeModelBuilder model, ExpressionStatementSyntax syntax)
        {
            return new ExpressionStatement {
                Expression = ExpressionReader.ReadExpression(model, syntax.Expression)
            };
        }
    }
}

#endif