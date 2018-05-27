namespace MetaPrograms.CodeModel.Imperative
{
    public interface ICodeGeneratorOutput
    {
        void AddSourceFile(string[] folderPath, string fileName, string contents);
    }
}
