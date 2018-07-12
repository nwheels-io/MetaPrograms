using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MetaPrograms.CodeModel.Imperative;
using MetaPrograms.CodeModel.Imperative.Expressions;

namespace MetaPrograms.Adapters.JavaScript.Writer
{
    public static class JavaScriptExpressionWriter
    {
        private static readonly Dictionary<Type, Action<CodeTextBuilder, AbstractExpression>> WriterByExpressionType =
            new Dictionary<Type, Action<CodeTextBuilder, AbstractExpression>> {
                [typeof(ConstantExpression)] = (c, e) => WriteConstant(c, (ConstantExpression)e),
                [typeof(TupleExpression)] = (c, e) => WriteTuple(c, (TupleExpression)e),
                [typeof(LocalVariableExpression)] = (c, e) => WriteVariable(c, (LocalVariableExpression)e),
                [typeof(ParameterExpression)] = (c, e) => WriteParameter(c, (ParameterExpression)e),
                [typeof(MemberExpression)] = (c, e) => WriteMember(c, (MemberExpression)e),
                [typeof(AnonymousDelegateExpression)] = (c, e) => WriteLambda(c, (AnonymousDelegateExpression)e),
                [typeof(MethodCallExpression)] = (c, e) => WriteMethodCall(c, (MethodCallExpression)e)
            };

        public static void WriteExpression(CodeTextBuilder code, AbstractExpression expression)
        {
            if (WriterByExpressionType.TryGetValue(expression.GetType(), out var writer))
            {
                writer(code, expression);
            }
            else
            {
                throw new NotSupportedException(
                    $"Expression of type '{expression.GetType().Name}' is not supported by {nameof(JavaScriptExpressionWriter)}.");
            }
        }

        public static void WriteConstant(CodeTextBuilder code, ConstantExpression constant)
        {
            JavaScriptLiteralWriter.WriteLiteral(code, constant.Value);
        }

        public static void WriteTuple(CodeTextBuilder code, TupleExpression tuple)
        {
            var variableListText = string.Join(", ", tuple.Variables.Select(v => v.Name));
            code.Write($"{{ {variableListText} }}");
        }

        public static void WriteVariable(CodeTextBuilder code, LocalVariableExpression variable)
        {
            code.Write(variable.VariableName ?? variable.Variable.Name);
        }

        public static void WriteParameter(CodeTextBuilder code, ParameterExpression expression)
        {
            if (expression.Parameter.Tuple != null)
            {
                WriteTuple(code, expression.Parameter.Tuple);
            }
            else
            {
                code.Write(expression.Parameter.Name);
            }
        }

        private static void WriteMember(CodeTextBuilder code, MemberExpression expression)
        {
            if (expression.Target != null)
            {
                WriteExpression(code, expression.Target);
                code.Write(".");
            }

            code.Write(expression.MemberName ?? expression.Member.Name);
        }

        private static void WriteLambda(CodeTextBuilder code, AnonymousDelegateExpression expression)
        {
            JavaScriptFunctionWriter.WriteArrowFunction(code, expression.Signature, expression.Body);
        }

        private static void WriteMethodCall(CodeTextBuilder code, MethodCallExpression call)
        {
            if (call.Target != null)
            {
                WriteExpression(code, call.Target);
                code.Write(".");
            }

            code.Write(call.MethodName ?? call.Method.Name);
            code.WriteListStart(opener: "(", separator: ", ", closer: ")");

            foreach (var argument in call.Arguments)
            {
                code.WriteListItem();
                WriteExpression(code, argument.Expression);
            }

            code.WriteListEnd();
        }
    }
}
