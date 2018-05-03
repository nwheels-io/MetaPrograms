using System;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Expressions
{
    public class Argument
    {
        public Argument(
            AbstractExpression expression, 
            MethodParameterModifier modifier)
        {
            this.Expression = expression;
            this.Modifier = modifier;
        }

        public Argument(
            Argument source, 
            Mutator<AbstractExpression>? expression = null,
            Mutator<MethodParameterModifier>? modifier = null)
        {
            this.Expression = expression.MutatedOrOriginal(source.Expression);
            this.Modifier = modifier.MutatedOrOriginal(source.Modifier);
        }

        public AbstractExpression Expression { get; }
        public MethodParameterModifier Modifier { get; }
    }
}
