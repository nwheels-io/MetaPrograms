using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MetaPrograms;
using MetaPrograms.Expressions;
using MetaPrograms.Members;
using MetaPrograms.Statements;

namespace MetaPrograms.JavaScript.Writer
{
    public static class JavaScriptFunctionWriter
    {
        public static void WriteFunction(CodeTextBuilder code, MethodMember method)
        {
            WriteModifiers(code, method);
            code.Write(method.Name.GetSealedOrCased(
                CasingStyle.Camel, 
                sealOrigin: IdentifierName.OriginKind.Generator, 
                sealLanguage: LanguageInfo.Entries.JavaScript()));

            WriteParameters(code, method.Signature);
            WriteBody(code, method.Body);
            code.WriteLine();
        }

        public static void WriteArrowFunction(CodeTextBuilder code, MethodSignature signature, BlockStatement body)
        {
            if (signature.IsAsync)
            {
                code.Write("async ");
            }
   
            if (signature.Parameters.Count != 1)
            {
                WriteParameters(code, signature);
            }
            else
            {
                code.Write($"{signature.Parameters[0].Name} ");
            }

            code.Write("=> ");

            switch (body.Statements.Count)
            {
                case 0:
                    code.Write("{ }");
                    break;
                case 1:
                    if (body.Statements[0] is ReturnStatement @return)
                    {
                        var needParentheses = (@return.Expression is ObjectInitializerExpression init && init.PropertyValues.Count > 1);
                        code.WriteIf(needParentheses, "(");                            
                        JavaScriptExpressionWriter.WriteExpression(code, @return.Expression);
                        code.WriteIf(needParentheses, ")");                            
                    }
                    else
                    {
                        JavaScriptStatementWriter.WriteStatement(code, body.Statements[0]);                        
                    }
                    break;
                default:
                    WriteBody(code, body);
                    break;
            }
        }

        private static void WriteModifiers(CodeTextBuilder code, MethodMember method)
        {
            if (method.Modifier == MemberModifier.Static)
            {
                code.Write("static ");
            }

            if (method.IsAsync)
            {
                code.Write("async ");
            }

            if (method.DeclaringType == null)
            {
                code.Write("function ");
            }
        }

        private static void WriteParameters(CodeTextBuilder code, MethodSignature signature)
        {
            code.WriteListStart(opener: "(", separator: ", ", closer: ") ");

            signature.Parameters.ForEach(parameter => {
                code.WriteListItem();

                if (parameter.Tuple != null)
                {
                    JavaScriptExpressionWriter.WriteTuple(code, parameter.Tuple);
                }
                else
                {
                    code.Write(parameter.Name);
                }
            });

            code.WriteListEnd();
        }

        private static void WriteBody(CodeTextBuilder code, BlockStatement body)
        {
            code.WriteLine("{");
            code.Indent(1);

            foreach (var statement in body.Statements)
            {
                JavaScriptStatementWriter.WriteStatementLine(code, statement);
            }

            code.Indent(-1);
            code.Write("}");
        }
    }
}
