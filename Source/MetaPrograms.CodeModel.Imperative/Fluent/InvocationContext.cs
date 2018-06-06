using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using MetaPrograms.CodeModel.Imperative.Expressions;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Fluent
{
    public class InvocationContext
    {
        private readonly List<Argument> _arguments = new List<Argument>();

        public void AddArgument(AbstractExpression expression, MethodParameterModifier modifier = MethodParameterModifier.None)
        {
            _arguments.Add(new Argument(expression, modifier));
        }

        public IReadOnlyList<Argument> GetArguments()
        {
            return _arguments;
        }
    }
}
