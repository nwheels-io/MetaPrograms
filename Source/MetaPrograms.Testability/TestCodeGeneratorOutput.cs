using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using MetaPrograms.Extensions;
using Shouldly;

namespace MetaPrograms.Testability
{
    public class TestCodeGeneratorOutput : ICodeGeneratorOutput
    {
        private readonly List<FileEntry> _sourceFiles = new List<FileEntry>();
        
        public void AddSourceFile(FilePath path, string contents)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            
            writer.Write(contents);
            writer.Flush();

            stream.Position = 0;
            _sourceFiles.Add(new FileEntry(path, stream));
        }

        public IDictionary<string, Stream> IndexSourceFilesByNormalPath()
        {
            return _sourceFiles.ToDictionary(f => f.Path.NormalizedFullPath, f => f.Contents);
        }

        public CodeTextOptions TextOptions => CodeTextOptions.Default;

        public IReadOnlyList<FileEntry> SourceFiles => _sourceFiles; 
        
        public void WriteToDisk(string baseFolderPath)
        {
            foreach (var entry in _sourceFiles)
            {
                var filePath = Path.Combine(baseFolderPath, entry.Path.FullPath);
                var folderPath = Path.GetDirectoryName(filePath);

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                using (var file = File.Create(filePath))
                {
                    entry.Contents.Position = 0;
                    entry.Contents.CopyTo(file);
                    file.Flush();
                }
            }
        }
        
        private void ShouldMatchFolder(string expectedOutputsFolder)
        {
            var expectedFilePaths =
                Directory.GetFiles(expectedOutputsFolder, "*.*", SearchOption.AllDirectories)
                    .Select(path => path.TrimSuffix(expectedOutputsFolder))
                    .Select(path => path.TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar))
                    .ToArray();

            var actualFilePaths =_sourceFiles.Select(f => f.Path.FullPath);
            actualFilePaths.ShouldBe(expectedFilePaths, ignoreOrder: true);
            
            foreach (var entry in _sourceFiles)
            {
                var expectedFilePath = Path.Combine(expectedOutputsFolder, entry.Path.FullPath);
                var actualStream = entry.Contents;
                actualStream.Position = 0;
                actualStream.ShouldMatchTextFile(expectedFilePath);
            }
        }

        public class FileEntry
        {
            public FileEntry(FilePath path, Stream contents)
            {
                Path = path;
                Contents = contents;
            }

            public readonly FilePath Path;
            public readonly Stream Contents;
        }
    }
}
