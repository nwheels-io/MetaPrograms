using System;
using System.Collections.Generic;
using System.Text;
using MetaPrograms.CodeModel.Imperative.Expressions;

namespace MetaPrograms.CodeModel.Imperative.Members
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

        public TypeMember FromModule { get; set; }
        public string FromModuleName { get; set; }
        public LocalVariable AsDefault { get; set; }
        public TupleExpression AsTuple { get; set; }
        public LocalVariable AsNamespace { get; set; }
    }
}
