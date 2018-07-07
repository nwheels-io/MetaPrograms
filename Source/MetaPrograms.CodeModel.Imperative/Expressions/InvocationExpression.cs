using System.Collections.Generic;
using System.Collections.Immutable;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Expressions
{
    public abstract class InvocationExpression : AbstractExpression
    {
        public List<Argument> Arguments { get; set; } = new List<Argument>();
    }
}
