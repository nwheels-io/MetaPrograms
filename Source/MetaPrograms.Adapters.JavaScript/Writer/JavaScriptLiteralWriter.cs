using System;
using System.Collections.Generic;
using System.Text;
using MetaPrograms.CodeModel.Imperative;

namespace MetaPrograms.Adapters.JavaScript.Writer
{
    public static class JavaScriptLiteralWriter
    {
        public static void WriteLiteral(CodeTextBuilder code, object value)
        {
            if (value == null)
            {
                code.Write("null");
            }
            else if (value is string @string)
            {
                WriteString(code, @string);
            }
            else if (value is bool @bool)
            {
                WriteBool(code, @bool);
            }
            else if (value is int @int)
            {
                WriteInteger(code, @int);
            }
            else if (value is float @float)
            {
                WriteFloat(code, @float);
            }
            else if (value is double @double)
            {
                WriteNumber(code, @double);
            }
            else if (value is Array @array)
            {
                WriteArray(code, @array);
            }
            else
            {
                throw new NotSupportedException($"Literal of type '{value.GetType().Name}' is not supported.");
            }
        }

        private static void WriteString(CodeTextBuilder code, string s)
        {
            code.Write("\"");
            code.Write(s.Replace("\"", "\\\""));
            code.Write("\"");
        }

        private static void WriteBool(CodeTextBuilder code, bool b)
        {
            throw new NotImplementedException();
        }

        private static void WriteInteger(CodeTextBuilder code, int i)
        {
            throw new NotImplementedException();
        }

        private static void WriteFloat(CodeTextBuilder code, float f)
        {
            throw new NotImplementedException();
        }

        private static void WriteNumber(CodeTextBuilder code, double d)
        {
            throw new NotImplementedException();
        }

        private static void WriteArray(CodeTextBuilder code, Array array)
        {
            throw new NotImplementedException();
        }
    }
}
