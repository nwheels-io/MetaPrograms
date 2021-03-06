﻿using System;
using System.Collections.Generic;
using System.Text;
using MetaPrograms;

namespace MetaPrograms.CSharp.Writer
{
    public static class IdentifierNameExtensions
    {
        public static string ToPascalCase(this IdentifierName identifier)
        {
            return identifier.GetSealedOrCased(
                CasingStyle.Pascal,
                sealLanguage: LanguageInfo.Entries.CSharp());
        }

        public static string ToCamelCase(this IdentifierName identifier)
        {
            return identifier.GetSealedOrCased(
                CasingStyle.Camel,
                sealLanguage: LanguageInfo.Entries.CSharp());
        }
    }
}
