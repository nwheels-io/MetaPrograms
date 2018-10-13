using System;
using System.Collections.Generic;

namespace MetaPrograms
{
    public interface ICodeGeneratorOutput
    {
        void AddSourceFile(IEnumerable<string> folderPath, string fileName, string contents);
        CodeTextOptions TextOptions { get; }
    }
}
