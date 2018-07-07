using System;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Expressions
{
    public class Argument
    {
        public AbstractExpression Expression { get; set; }
        public MethodParameterModifier Modifier { get; set; }
    }
}
