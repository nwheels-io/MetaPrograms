using System.Text;

namespace MetaPrograms
{
    public class StringCodeGeneratorOutput : ICodeGeneratorOutput
    {
        private readonly StringBuilder _output = new StringBuilder();
        
        public StringCodeGeneratorOutput(CodeTextOptions textOptions)
        {
            TextOptions = textOptions;
        }

        public void AddSourceFile(FilePath path, string contents)
        {
            _output.AppendLine(contents);
        }

        public CodeTextOptions TextOptions { get; }

        public string GetString() => _output.ToString();
    }
}
