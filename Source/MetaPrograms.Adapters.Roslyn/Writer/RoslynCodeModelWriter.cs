using MetaPrograms.CodeModel.Imperative;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.Adapters.Roslyn.Writer
{
    public class RoslynCodeModelWriter
    {
        private readonly ICodeGeneratorOutput _output;

        public RoslynCodeModelWriter(ICodeGeneratorOutput output)
        {
            _output = output;
        }

        public void AddCodeModel(ImmutableCodeModel codeModel)
        {
            
        }

        public void WriteAll()
        {
            
        }
    }
}