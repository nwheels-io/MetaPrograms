using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MetaPrograms.CodeModel.Imperative;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.Adapters.JavaScript.Writer
{
    public static class JavaScriptImportWriter
    {
        public static void WriteImport(CodeTextBuilder code, ImportDirective import)
        {
            code.Write("import ");
            
            if (import.AsDefault != null)
            {
                code.Write(import.AsDefault.Name);
            }
            else if (import.AsNamespace != null)
            {
                code.Write($"* as {import.AsNamespace.Name}");
            }
            else if (import.AsTuple != null)
            {
                JavaScriptExpressionWriter.WriteTuple(code, import.AsTuple);
            }

            if (import.From != null)
            {
                code.Write($" from '{import.From.GetModulePath()}'");
            }
            else if (import.What != null)
            {
                code.Write($"'{import.What.GetModulePath()}'");
            }            

            code.WriteLine(";");
        }
    }
}
