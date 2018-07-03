using System;
using System.Collections.Generic;
using System.Text;
using MetaPrograms.CodeModel.Imperative.Expressions;

namespace MetaPrograms.CodeModel.Imperative.Members
{
    public class ImportDirective
    {
        public ImportDirective(
            MemberRef<TypeMember> fromModule,
            string fromModuleName,
            LocalVariable asDefault = null,
            TupleExpression asTuple = null, 
            LocalVariable asNamespace = null)
        {
            FromModule = fromModule;
            FromModuleName = fromModuleName;
            AsDefault = asDefault;
            AsTuple = asTuple;
            AsNamespace = asNamespace;
        }

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

        public MemberRef<TypeMember> FromModule { get; }
        public string FromModuleName { get; }
        public LocalVariable AsDefault { get; }
        public TupleExpression AsTuple { get; }
        public LocalVariable AsNamespace { get; }
    }
}
