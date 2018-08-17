using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace MetaPrograms.CodeModel.Imperative
{
    public class IdentifierName : IEquatable<IdentifierName>
    {
        private static readonly string UnderscoreSeparator = "_";
        private static readonly string HyphenSeparator = "-";

        public string OriginalName { get; }
        public LanguageInfo OriginalLanguage { get; }
        public OriginKind Origin { get; }
        public ImmutableList<Fragment> Fragments { get; }

        private readonly string _comparisonString;

        public IdentifierName(string name, LanguageInfo language, OriginKind origin = OriginKind.Generator, CasingStyle? style = null)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException(nameof(name));
            }

            this.OriginalName = name;
            this.OriginalLanguage = language;
            this.Origin = origin;
            this.Fragments = ParseFragments(name, style).Select(f => new Fragment(f)).ToImmutableList();

            _comparisonString = MakeComparisonString(Fragments);
        }

        public IdentifierName(IEnumerable<string> fragments, OriginKind origin = OriginKind.Generator, LanguageInfo language = null)
            : this(fragments.Select(f => new Fragment(f)), origin, language)
        {
        }

        public IdentifierName(IEnumerable<Fragment> fragments, OriginKind origin = OriginKind.Generator, LanguageInfo language = null)
        {
            this.Fragments = fragments.ToImmutableList();

            this.OriginalName = string.Join("", this.Fragments.Select(f => f.OriginalText));
            this.OriginalLanguage = language ?? LanguageInfo.Current;
            this.Origin = origin;

            _comparisonString = MakeComparisonString(Fragments);
        }

        public IdentifierName(object[] anyFragments)
        {
            var fragmentList = GetFlatListOfFragments(anyFragments);
            this.Fragments = fragmentList.ToImmutableList();

            this.Origin = OriginKind.Generator;
            this.OriginalName = string.Join("", fragmentList.Select(f => f.OriginalText));
            this.OriginalLanguage = null;

            _comparisonString = MakeComparisonString(Fragments);
        }

        public override string ToString()
        {
            return OriginalName;
        }

        public string GetSealedOrCased(CasingStyle casing, OriginKind? sealOrigin = null, LanguageInfo sealLanguage = null)
        {
            if (IsOriginalNameSealed(sealOrigin, sealLanguage))
            {
                return OriginalName;
            }

            return ToString(casing);
        }

        public IdentifierName AppendPrefixFragments(params string[] fragments)
        {
            return new IdentifierName(new object[] { fragments, this });
        }

        public IdentifierName AppendSuffixFragments(params string[] fragments)
        {
            return new IdentifierName(new object[] { this, fragments });
        }

        public IdentifierName TrimPrefixFragment(string fragment)
        {
            if (Fragments.Count > 1 && Fragments[0].Text.Equals(fragment, StringComparison.InvariantCultureIgnoreCase))
            {
                return new IdentifierName(Fragments.Skip(1), this.Origin, this.OriginalLanguage);
            }

            return this;
        }

        public IdentifierName TrimSuffixFragment(string fragment)
        {
            if (Fragments.Count > 1 && Fragments[Fragments.Count - 1].Text.Equals(fragment, StringComparison.InvariantCultureIgnoreCase))
            {
                return new IdentifierName(Fragments.Take(Fragments.Count - 1), this.Origin, this.OriginalLanguage);
            }

            return this;
        }

        public string ToString(CasingStyle style)
        {
            var separator = GetSeparator(style);
            var result = new StringBuilder();

            for (int i = 0; i < Fragments.Count; i++)
            {
                result.Append(Fragments[i].ToString(style, first: i == 0));

                if (i < Fragments.Count - 1)
                {
                    result.Append(separator);
                }
            }

            return result.ToString();
        }

        public override int GetHashCode()
        {
            return _comparisonString.GetHashCode();
        }

        public bool Equals(IdentifierName other)
        {
            return (this._comparisonString == other?._comparisonString);
        }

        public override bool Equals(object obj)
        {
            if (obj is IdentifierName other)
            {
                return Equals(other);
            }

            return false;
        }

        public static bool operator ==(IdentifierName a, IdentifierName b)
        {
            if (ReferenceEquals(a, null))
            {
                return ReferenceEquals(b, null);
            }

            return a.Equals(b);
        }

        public static bool operator !=(IdentifierName a, IdentifierName b)
        {
            return !(a == b);
        }

        public string SourceName => (Origin == OriginKind.Source ? OriginalName : null);
        public string GeneratedName => (Origin == OriginKind.Generator ? OriginalName : null);

        private bool IsOriginalNameSealed(OriginKind? origin, LanguageInfo language)
        {
            if (OriginalName == null)
            {
                return false;
            }

            if (origin.HasValue && origin.Value != Origin)
            {
                return false;
            }

            if (language != null && language != OriginalLanguage)
            {
                return false;
            }

            return true;
        }

        public static implicit operator IdentifierName(string name)
        {
            if (name == null)
            {
                return null;
            }

            var context = CodeContextBase.TryGetContext<CodeContextBase>();
            return new IdentifierName(name, context?.Language, context?.DefaultIdentifierOrigin ?? OriginKind.Generator);
        }

        public static implicit operator string(IdentifierName identifier)
        {
            return identifier.ToString();
        }

        private static List<Fragment> GetFlatListOfFragments(object[] anyFragments)
        {
            var fragmentList = new List<Fragment>();

            foreach (var obj in anyFragments)
            {
                switch (obj)
                {
                    case null:
                        break;
                    case string s:
                        fragmentList.Add(new Fragment(s));
                        break;
                    case Fragment fragment:
                        fragmentList.Add(fragment);
                        break;
                    case IdentifierName identifier:
                        fragmentList.AddRange(identifier.Fragments);
                        break;
                    case IEnumerable<Fragment> fragmentEnumerable:
                        fragmentList.AddRange(fragmentEnumerable);
                        break;
                    case IEnumerable<string> stringEnumerable:
                        fragmentList.AddRange(stringEnumerable.Select(s => new Fragment(s)));
                        break;
                    default:
                        fragmentList.Add(new Fragment(obj.ToString()));
                        break;
                }
            }

            return fragmentList;
        }

        private static IEnumerable<string> ParseFragments(string s, CasingStyle? style = null)
        {
            var effectiveStyle = style ?? GuessCasingStyle(s);

            if (effectiveStyle == CasingStyle.Pascal || effectiveStyle == CasingStyle.Camel)
            {
                return ParseCamelCaseFragments(s);
            }

            var separator = GetSeparator(effectiveStyle);
            var fragments = s.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries);

            return fragments;
        }

        private static IEnumerable<string> ParseCamelCaseFragments(string name)
        {
            var fragments = new List<string>();
            var word = new StringBuilder();

            void flushWord()
            {
                if (word.Length > 0)
                {
                    fragments.Add(word.ToString());
                    word.Clear();
                }
            }

            word.Append(name[0]);

            for (int i = 1; i < name.Length; i++)
            {
                bool isNewWord = false;

                if (char.IsDigit(name[i]))
                {
                    isNewWord = !char.IsDigit(name[i - 1]);
                }
                else if (char.IsUpper(name[i]))
                {
                    isNewWord = (!char.IsUpper(name[i - 1]) || (i < name.Length - 1 && char.IsLower(name[i + 1])));
                }

                if (isNewWord)
                {
                    flushWord();
                }

                word.Append(name[i]);
            }

            flushWord();
            return fragments;
        }

        private static CasingStyle GuessCasingStyle(string name)
        {
            var counters = new CharCounters();
            CountChars(name, ref counters);

            if (counters.HyphenCount > 0)
            {
                return CasingStyle.Kebab;
            }

            if (counters.UnderscoreCount > 0)
            {
                return (counters.LowerLetterCount > 0 ? CasingStyle.Snake : CasingStyle.ScreamingCaps);
            }

            if (counters.LowerLetterCount == 0 && counters.UpperLetterCount > 1)
            {
                return CasingStyle.ScreamingCaps;
            }

            return (counters.IsFirstCharUpperLetter ? CasingStyle.Pascal : CasingStyle.Camel);
        }

        private static void CountChars(string name, ref CharCounters counters)
        {
            for (int i = 0; i < name.Length; i++)
            {
                var c = name[i];

                if (char.IsUpper(c))
                {
                    counters.UpperLetterCount++;
                    if (i == 0)
                    {
                        counters.IsFirstCharUpperLetter = true;
                    }
                }
                else if (char.IsLower(c))
                {
                    counters.LowerLetterCount++;
                }
                else if (char.IsDigit(c))
                {
                    counters.DigitCount++;
                }
                else if (c == '-')
                {
                    counters.HyphenCount++;
                }
                else if (c == '_')
                {
                    counters.UnderscoreCount++;
                }
            }
        }

        private static string GetSeparator(CasingStyle style)
        {
            switch (style)
            {
                case CasingStyle.Kebab:
                    return HyphenSeparator;
                case CasingStyle.Snake:
                case CasingStyle.ScreamingCaps:
                    return UnderscoreSeparator;
                case CasingStyle.UserFriendly:
                    return " ";
;                default:
                    return string.Empty;
            }
        }

        private static string MakeComparisonString(IImmutableList<Fragment> fragments)
        {
            return string.Concat(fragments.Select(f => f.Text));
        }

        public enum OriginKind
        {
            Source,
            Generator
        }

        public readonly struct Fragment
        {
            public Fragment(string text)
            {
                this.OriginalText = text;
                this.Text = text.ToLower();
            }

            public readonly string OriginalText;
            public readonly string Text;

            public override string ToString()
            {
                return Text;
            }

            public string ToString(CasingStyle style, bool first)
            {
                switch (style)
                {
                    case CasingStyle.Kebab:
                    case CasingStyle.Snake:
                        return Text;
                    case CasingStyle.Camel:
                        return (first ? Text : char.ToUpper(Text[0]) + Text.Substring(1));
                    case CasingStyle.Pascal:
                        return char.ToUpper(Text[0]) + Text.Substring(1);
                    case CasingStyle.ScreamingCaps:
                        return Text.ToUpper();
                    case CasingStyle.UserFriendly:
                        return (first ? char.ToUpper(Text[0]) + Text.Substring(1) : Text);
                }

                throw new ArgumentOutOfRangeException(nameof(style));
            }
        }

        private struct CharCounters
        {
            public bool IsFirstCharUpperLetter;
            public int UpperLetterCount;
            public int LowerLetterCount;
            public int DigitCount;
            public int HyphenCount;
            public int UnderscoreCount;
        }
    }
}
