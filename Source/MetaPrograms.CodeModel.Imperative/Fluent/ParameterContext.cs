using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Fluent
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
