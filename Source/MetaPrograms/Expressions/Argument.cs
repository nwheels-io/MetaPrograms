using System;
using MetaPrograms.Members;

namespace MetaPrograms.Expressions
{
    public class Argument
    {
        public AbstractExpression Expression { get; set; }
        public MethodParameterModifier Modifier { get; set; }
    }
}
