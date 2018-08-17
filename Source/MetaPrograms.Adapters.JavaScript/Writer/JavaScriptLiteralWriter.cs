using System;
using System.Collections.Generic;
using System.Globalization;
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

        private static void WriteBool(CodeTextBuilder code, bool value)
        {
            code.Write(value ? "true" : "false");
        }

        private static void WriteInteger(CodeTextBuilder code, int value)
        {
            code.Write(value.ToString());
        }

        private static void WriteFloat(CodeTextBuilder code, float value)
        {
            code.Write(value.ToString(CultureInfo.InvariantCulture));
        }

        private static void WriteNumber(CodeTextBuilder code, double value)
        {
            code.Write(value.ToString(CultureInfo.InvariantCulture));
        }

        private static void WriteArray(CodeTextBuilder code, Array array)
        {
            code.WriteListStart(opener: "[", closer: "]", separator: ",");

            foreach (var item in array)
            {
                code.WriteListItem();
                WriteLiteral(code, item);
            }

            code.WriteListEnd();
        }
    }
}
