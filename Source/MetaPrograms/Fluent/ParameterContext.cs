using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using MetaPrograms.Members;

namespace MetaPrograms.Fluent
{
    public class ParameterContext
    {
        public ParameterContext(MethodParameter parameter)
        {
            this.Parameter = parameter;
        }

        public MethodParameter Parameter { get; }
    }
}
