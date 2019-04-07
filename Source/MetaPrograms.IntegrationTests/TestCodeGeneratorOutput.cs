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

        public void AddSourceFile(FilePath path, string contents)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            
            writer.Write(contents);
            writer.Flush();

            SourceFiles = SourceFiles.Add(path.NormalizedFullPath, stream);
        }

        public CodeTextOptions TextOptions => CodeTextOptions.Default;

        public ImmutableDictionary<string, Stream> SourceFiles { get; private set; }
    }
}
