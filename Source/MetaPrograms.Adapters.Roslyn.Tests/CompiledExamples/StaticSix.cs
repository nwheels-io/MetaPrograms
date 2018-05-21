using System;
using System.Collections.Generic;
using System.Text;

namespace MetaPrograms.Adapters.Roslyn.Tests.CompiledExamples
{
    public static class StaticSix
    {
        public static void M1()
        {
        }

        public static T2 M2<T1, T2>()
        {
            return default(T2);
        }

        public static IInterfaceOne P1 { get; set; }
    }
}
