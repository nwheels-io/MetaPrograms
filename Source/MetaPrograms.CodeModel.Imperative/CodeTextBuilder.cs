using System;
using System.Data;
using System.Linq;
using System.Text;
using MetaPrograms.CodeModel.Imperative.Fluent;

namespace MetaPrograms.CodeModel.Imperative
{
    public class CodeTextBuilder
    {
        private readonly CodeTextOptions _options;
        private readonly StringBuilder _text;
        private int _indentLevel = 0;
        
        public CodeTextBuilder(CodeTextOptions options)
        {
            _options = options;
            _text = new StringBuilder();
        }

        public void Write(string s)
        {
            _text.Append(s);
        }

        public void WriteLine()
        {
            WriteLine(string.Empty);
        }

        public void WriteLine(string s)
        {
            _text.AppendLine(s);
            WriteIndent(_indentLevel);
        }

        public void Indent(int delta = 1)
        {
            var oldLevel = _indentLevel;
            _indentLevel += delta;

            if (_indentLevel < 0)
            {
                _indentLevel = 0;
            }

            ChangeLastLineIndent(deltaLevel: _indentLevel - oldLevel);
        }

        public override string ToString()
        {
            return _text.ToString();
        }

        public int IndentLevel => _indentLevel;
        
        private void ChangeLastLineIndent(int deltaLevel)
        {
            if (deltaLevel > 0)
            {
                WriteIndent(deltaLevel);
            }
            else
            {
                var lengthToRemove = deltaLevel * _options.Indent.Length;
                int newLength = _text.Length;

                for (int i = 0; i < lengthToRemove && newLength > 0; i++, newLength--)
                {
                    var c = _text[newLength - 1];
                    if (c == '\r' || c == '\n' || !char.IsWhiteSpace(c))
                    {
                        break;
                    }
                }

                _text.Length = newLength;
            }
        }

        private void WriteIndent(int times)
        {
            _text.Append(string.Concat(Enumerable.Repeat(_options.Indent, times)));
        }
    }
}