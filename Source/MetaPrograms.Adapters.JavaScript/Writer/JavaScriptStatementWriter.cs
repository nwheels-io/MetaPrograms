using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using MetaPrograms.CodeModel.Imperative;
using MetaPrograms.CodeModel.Imperative.Statements;

namespace MetaPrograms.Adapters.JavaScript.Writer
{
    public static class JavaScriptStatementWriter
    {
        private static readonly Dictionary<Type, Action<CodeTextBuilder, AbstractStatement>> WriterByStatementType =
            new Dictionary<Type, Action<CodeTextBuilder, AbstractStatement>> {
                [typeof(ExpressionStatement)] = (c, s) => WriteExpression(c, (ExpressionStatement)s),
                [typeof(ReturnStatement)] = (c, s) => WriteReturn(c, (ReturnStatement)s)
            };

        public static void WriteStatementLine(CodeTextBuilder code, AbstractStatement statement)
        {
            WriteStatement(code, statement);
            code.WriteLine(";");
        }

        public static void WriteStatement(CodeTextBuilder code, AbstractStatement statement)
        {
            if (WriterByStatementType.TryGetValue(statement.GetType(), out var writer))
            {
                writer(code, statement);
            }
            else
            {
                throw new NotSupportedException(
                    $"Statement of type '{statement.GetType().Name}' is not supported by {nameof(JavaScriptStatementWriter)}.");
            }
        }

        private static void WriteExpression(CodeTextBuilder code, ExpressionStatement statement)
        {
            JavaScriptExpressionWriter.WriteExpression(code, statement.Expression);
        }

        private static void WriteReturn(CodeTextBuilder code, ReturnStatement statement)
        {
            code.Write("return ");
            JavaScriptExpressionWriter.WriteExpression(code, statement.Expression);
        }
    }
}
