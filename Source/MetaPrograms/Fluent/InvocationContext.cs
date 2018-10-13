using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using MetaPrograms.Expressions;
using MetaPrograms.Members;

namespace MetaPrograms.Fluent
{
    public class InvocationContext
    {
        private readonly List<Argument> _arguments = new List<Argument>();

        public void AddArgument(AbstractExpression expression, MethodParameterModifier modifier = MethodParameterModifier.None)
        {
            _arguments.Add(new Argument { 
                Expression = expression, 
                Modifier = modifier
            });
        }

        public IReadOnlyList<Argument> GetArguments()
        {
            return _arguments;
        }
    }
}
