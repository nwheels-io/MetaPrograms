using System;
using System.Collections.Generic;
using System.Text;

namespace MetaPrograms.Adapters.Roslyn.Tests.CompiledExamples
{
    public class SeventhAttribute : Attribute
    {
        public SeventhAttribute(int number, DayOfWeek day)
        {
            Number = number;
            Day = day;
        }

        public int Number { get; }
        public DayOfWeek Day { get; }
        public string Text { get; set; }
        public EnumNine Nine { get; set; }
    }
}
