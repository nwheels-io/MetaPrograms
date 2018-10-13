using System;

namespace MetaPrograms
{
    public class CodeTextOptions
    {
        public CodeTextOptions(string indent, string newLine)
        {
            this.Indent = indent;
            this.NewLine = newLine;
        }

        public string Indent { get; }
        public string NewLine { get; }

        public static readonly CodeTextOptions Default = new CodeTextOptions(
            indent: new string(' ', 4),
            newLine: Environment.NewLine);
    }
}
