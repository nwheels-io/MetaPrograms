using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using MetaPrograms.Expressions;

namespace MetaPrograms.Members
{
    public class ImportDirective
    {
        public IEnumerable<LocalVariable> GetLocalVariables()
        {
            var result = new List<LocalVariable>();

            if (AsDefault != null)
            {
                result.Add(AsDefault);
            }

            if (AsNamespace != null)
            {
                result.Add(AsNamespace);
            }

            if (AsTuple != null)
            {
                result.AddRange(AsTuple.Variables);
            }

            return result;
        }

        public ModuleSpecifier What { get; set; }
        public ModuleSpecifier From { get; set; }
        public LocalVariable AsDefault { get; set; }
        public TupleExpression AsTuple { get; set; }
        public LocalVariable AsNamespace { get; set; }

        public class ModuleSpecifier
        {
            public ModuleMember Module { get; set; }
            public string ModulePath { get; set; }

            public string GetModulePath()
            {
                if (!string.IsNullOrEmpty(ModulePath))
                {
                    return ModulePath;
                }
                else if (Module != null)
                {
                    return string.Join("/", GetModulePathParts(Module));
                }

                throw new InvalidCodeModelException(
                    $"Import directive module specifier has neither Module nor ModulePath.");
            }

            private static IEnumerable<string> GetModulePathParts(ModuleMember module)
            {
                var pathParts = new List<string>();

                pathParts.Add(".");
                
                if (module.FolderPath != null)
                {
                    pathParts.AddRange(module.FolderPath);
                }

                pathParts.Add(module.Name.ToString());
                
                return pathParts;
            }
        }
    }
}
