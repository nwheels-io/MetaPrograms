using System;
using System.Collections.Generic;
using System.Text;

namespace MetaPrograms.Adapters.Roslyn.Tests.CompiledExamples
{
    public class EighthAttribute : Attribute
    {
        public EnumTens[] Tens { get; set; }
    }
}
