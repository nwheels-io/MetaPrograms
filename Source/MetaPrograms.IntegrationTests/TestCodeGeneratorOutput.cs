using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using MetaPrograms;

namespace MetaPrograms.IntegrationTests
{
    public class TestCodeGeneratorOutput : ICodeGeneratorOutput
    {
        public TestCodeGeneratorOutput()
        {
            SourceFiles = ImmutableDictionary<string, Stream>.Empty;
        }

        public void AddSourceFile(IEnumerable<string> folderPath, string fileName, string contents)
        {
            var filePath = Path
                .Combine(folderPath.Concat(new[] { fileName }).ToArray())
                .Replace(Path.DirectorySeparatorChar, '/');

            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            
            writer.Write(contents);
            writer.Flush();

            SourceFiles = SourceFiles.Add(filePath, stream);
        }

        public CodeTextOptions TextOptions => CodeTextOptions.Default;

        public ImmutableDictionary<string, Stream> SourceFiles { get; private set; }
    }
}
