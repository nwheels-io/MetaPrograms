using System;
using System.Collections.Generic;

namespace MetaPrograms
{
    public interface ICodeGeneratorOutput
    {
        void AddSourceFile(FilePath path, string contents);
        CodeTextOptions TextOptions { get; }
    }

    public static class CodeGeneratorOutputExtensions
    {
        public static void AddSourceFile(this ICodeGeneratorOutput output, IEnumerable<string> folderPath, string fileName, string contents)
        {
            output.AddSourceFile(new FilePath(folderPath, fileName), contents);
        }
    }
}
