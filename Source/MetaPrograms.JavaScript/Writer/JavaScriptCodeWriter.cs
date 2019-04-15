using System.Linq;
using MetaPrograms;
using MetaPrograms.Members;

namespace MetaPrograms.JavaScript.Writer
{
    public class JavaScriptCodeWriter
    {
        private readonly ICodeGeneratorOutput _output;
        private CodeTextBuilder _code;

        public JavaScriptCodeWriter(ICodeGeneratorOutput output)
        {
            _output = output;
        }
        
        public void WriteModule(ModuleMember module, bool privateScope = false)
        {
            _code = new CodeTextBuilder(_output.TextOptions);

            WriteImports(module);

            if (privateScope)
            {
                _code.WriteLine("(function() {");
                _code.Indent(1);
            }
            
            WriteMembers(module);
            WriteGlobalBlock(module);

            if (privateScope)
            {
                _code.Indent(-1);
                _code.WriteLine("}());");
            }

            _output.AddSourceFile(module.FolderPath, $"{module.Name.ToString(CasingStyle.Kebab)}.js", _code.ToString());
        }

        public CodeTextBuilder Code => _code;

        private void WriteImports(ModuleMember module)
        {
            foreach (var import in module.Imports)
            {
                JavaScriptImportWriter.WriteImport(_code, import);
            }
        }

        private void WriteMembers(ModuleMember module)
        {
            foreach (var member in module.Members)
            {
                _code.WriteLine();
                JavaScriptMemberWriter.WriteMember(_code, member);
            }
        }

        private void WriteGlobalBlock(ModuleMember module)
        {
            if (module.GloalBlock != null)
            {
                JavaScriptStatementWriter.WriteStatementLine(_code, module.GloalBlock);
            }
        }

    }
}
