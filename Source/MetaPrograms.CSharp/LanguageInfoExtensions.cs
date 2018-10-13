using System;
using System.Collections.Generic;
using System.Text;
using MetaPrograms;

namespace MetaPrograms.CSharp
{
    public static class LanguageInfoExtensions
    {
        private static readonly LanguageInfo CSharpLanguage = new LanguageInfo(name: "C#", abbreviation: "CS");

        public static LanguageInfo CSharp(this LanguageInfo.ExtensibleEntries entries) => CSharpLanguage;
    }
}
