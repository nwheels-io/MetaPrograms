using System.Linq;
using MetaPrograms.CodeModel.Imperative;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.Adapters.JavaScript.Writer
{
    public class JavaScriptCodeWriter
    {
        private readonly ICodeGeneratorOutput _output;
        private CodeTextBuilder _code;

        public JavaScriptCodeWriter(ICodeGeneratorOutput output)
        {
            _output = output;
        }
        
        public void WriteModule(ModuleMember module)
        {
            _code = new CodeTextBuilder(_output.TextOptions);

            WriteImports(module);
            WriteMembers(module);
            
            _output.AddSourceFile(module.FolderPath, $"{module.Name}.js", _code.ToString());
        }

        private void WriteImports(ModuleMember module)
        {
            foreach (var import in module.Imports)
            {
                Write(import);
            }
        }

        private void WriteMembers(ModuleMember module)
        {
            foreach (var member in module.Members)
            {
                _code.WriteLine();
                WriteMember(member);
            }
        }

        private void WriteMember(AbstractMember member)
        {
            if (member is TypeMember type)
            {
                WriteType(type);
            }
            else if (member is MethodMember method)
            {
                WriteMethod(method);
            }
            else if (member is FieldMember field)
            {
                WriteField(field);
            }
        }

        private void WriteType(TypeMember type)
        {
            
            _code.WriteLine("");
        }

        private void WriteMethod(MethodMember method)
        {
            _code.WriteLine($"/* FUNCTION: {method.Name}  */");
        }

        private void WriteField(FieldMember field)
        {
            _code.WriteLine($"/* FIELD: {field.Name}  */");
        }

        private void Write(ImportDirective import)
        {
            var fromName = import.FromModuleName ?? import.FromModule.Name;

            if (import.AsDefault != null)
            {
                _code.Write($"import {import.AsDefault.Name}");
            }
            else if (import.AsNamespace != null)
            {
                _code.Write($"import * as {import.AsNamespace.Name}");
            }
            else if (import.AsTuple != null)
            {
                var variableListText = string.Join(", ", import.AsTuple.Variables.Select(v => v.Name));
                _code.Write($"import {{{variableListText}}}");
            }
            
            _code.WriteLine($" from '{fromName}';");
        }
    }
}
