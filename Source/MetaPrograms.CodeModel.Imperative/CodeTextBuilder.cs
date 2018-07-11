using System;
using System.Collections.Generic;
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
        private readonly Stack<ListState> _listStack = new Stack<ListState>();
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

        public void WriteListStart(string separator = ", ", string opener = null, string closer = null, bool newLine = false)
        {
            _listStack.Push(new ListState(opener, closer, separator, newLine));

            if (opener != null)
            {
                Write(opener);
            }

            if (newLine)
            {
                WriteLine();
                Indent(1);
            }
        }

        public void WriteListItem()
        {
            WriteListItem(string.Empty);
        }

        public void WriteListItem(string s)
        {
            var list = _listStack.Peek();

            if (++list.Index > 0)
            {
                Write(list.Separator);

                if (list.NewLine)
                {
                    WriteLine();
                }
            }

            Write(s);
        }

        public void WriteListEnd()
        {
            var list = _listStack.Pop();

            if (list.NewLine)
            {
                WriteLine();
                Indent(-1);
            }

            if (list.Closer != null)
            {
                Write(list.Closer);
            }
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
                var lengthToRemove = (-deltaLevel) * _options.Indent.Length;
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

        private class ListState
        {
            public ListState(string opener, string closer, string separator, bool newLine)
            {
                Opener = opener;
                Closer = closer;
                Separator = separator;
                NewLine = newLine;
                Index = -1;
            }

            public string Opener { get; }
            public string Closer { get; }
            public string Separator { get; }
            public bool NewLine { get; }
            public int Index { get; set; }
        }
    }
}