using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Fluent
{
    public class ParameterContext
    {
        public ParameterContext()
        {
            Modifier = MethodParameterModifier.None;
            Attributes = ImmutableList<AttributeDescription>.Empty;
        }

        public MethodParameterModifier Modifier { get; set; }
        public ImmutableList<AttributeDescription> Attributes { get; set; }
    }
}
