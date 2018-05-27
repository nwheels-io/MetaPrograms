using System.Collections.Immutable;
using System.IO;
using System.Linq;
using MetaPrograms.CodeModel.Imperative;

namespace MetaPrograms.IntegrationTests
{
    public class TestCodeGeneratorOutput : ICodeGeneratorOutput
    {
        public TestCodeGeneratorOutput()
        {
            SourceFiles = ImmutableDictionary<string, Stream>.Empty;
        }

        public void AddSourceFile(string[] folderPath, string fileName, string contents)
        {
            var filePath = string.Join('/', folderPath.Concat(new[] {fileName}));
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            
            writer.Write(contents);
            writer.Flush();

            SourceFiles = SourceFiles.Add(filePath, stream);
        }
        
        public ImmutableDictionary<string, Stream> SourceFiles { get; private set; }
    }
}