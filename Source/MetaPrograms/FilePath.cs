using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MetaPrograms
{
    public class FilePath
    {
        private readonly string[] _subFolder;
        private readonly string _fileName;

        public FilePath(params string[] pathAndName)
        {
            _subFolder = pathAndName.Take(pathAndName.Length - 1).ToArray();
            _fileName = pathAndName.Last();
        }

        public FilePath(IEnumerable<string> subFolder, string fileName)
        {
            _subFolder = subFolder.ToArray();
            _fileName = fileName;
        }

        public FilePath WithBase(string basePath)
        {
            return new FilePath(new[] { basePath }.Concat(_subFolder), _fileName);
        }

        public FilePath Append(params string[] pathAndName)
        {
            return new FilePath(_subFolder.Append(_fileName).Concat(pathAndName).ToArray());
        }
        
        public IReadOnlyList<string> SubFolder => _subFolder;
        public string FileName => _fileName;
        public string FolderPath => Path.Combine(_subFolder);
        public string FullPath => Path.Combine(FolderPath, _fileName);
        
        public string NormalizedFolderPath => FolderPath.Replace(Path.DirectorySeparatorChar, '/');
        public string NormalizedFullPath => FullPath.Replace(Path.DirectorySeparatorChar, '/');
    }
}
