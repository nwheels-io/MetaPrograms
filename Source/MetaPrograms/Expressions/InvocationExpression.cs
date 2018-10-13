using System.Collections.Generic;
using System.Collections.Immutable;
using MetaPrograms.Members;

namespace MetaPrograms.Expressions
{
    public abstract class InvocationExpression : AbstractExpression
    {
        public List<Argument> Arguments { get; set; } = new List<Argument>();
    }
}
