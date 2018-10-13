using System;
using System.Collections.Generic;
using System.Text;

namespace MetaPrograms
{
    public class LanguageInfo
    {
        public LanguageInfo(string name, string abbreviation)
        {
            Name = name;
            Abbreviation = abbreviation;
        }

        public string Name { get; }
        public string Abbreviation { get; }

        public static LanguageInfo Current => CodeContextBase.GetContextOrThrow<CodeContextBase>().Language;
        public static ExtensibleEntries Entries => ExtensibleEntries.Instance;

        public class ExtensibleEntries
        {
            private ExtensibleEntries()
            {
            }

            public static readonly ExtensibleEntries Instance = new ExtensibleEntries();
        }
    }
}
