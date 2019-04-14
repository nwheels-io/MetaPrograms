using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using MetaPrograms;

namespace MetaPrograms.JavaScript.Writer
{
    public static class JavaScriptLiteralWriter
    {
        private static readonly Dictionary<char, string> CharacterEscapes =
            new Dictionary<char, string> {
                { '\\', "\\\\" },
                { '\r', "\\r" },
                { '\n', "\\n" },
                { '\t', "\\t" },
                { '"', "\\\"" },
            };

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
            var result = new StringBuilder(capacity: s.Length + 4);
            result.Append("\"");

            for (int i = 0; i < s.Length; i++)
            {
                if (CharacterEscapes.TryGetValue(s[i], out var escape))
                {
                    result.Append(escape);
                }
                else
                {
                    result.Append(s[i]);
                }
            }
            
            result.Append("\"");
            code.Write(result.ToString());
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
