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
            var fromName = import.FromModuleName ?? import.FromModule.Name;

            if (import.AsDefault != null)
            {
                code.Write($"import {import.AsDefault.Name}");
            }
            else if (import.AsNamespace != null)
            {
                code.Write($"import * as {import.AsNamespace.Name}");
            }
            else if (import.AsTuple != null)
            {
                JavaScriptExpressionWriter.WriteTuple(code, import.AsTuple);
            }

            code.WriteLine($" from '{fromName}';");
        }
    }
}
