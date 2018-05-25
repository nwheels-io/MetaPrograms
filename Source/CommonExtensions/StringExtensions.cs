namespace CommonExtensions
{
    public static class StringExtensions
    {
        public static string TrimPrefix(this string s, string prefix)
        {
            if (s != null && s.Length > prefix.Length && s.StartsWith(prefix))
            {
                return s.Substring(prefix.Length);
            }

            return s;
        }

        public static string TrimSuffix(this string s, string suffix)
        {
            if (s != null && s.Length > suffix.Length && s.EndsWith(suffix))
            {
                return s.Substring(0, s.Length - suffix.Length);
            }

            return s;
        }

        public static string ToCamelCase(this string s)
        {
            if (s?.Length > 0)
            {
                return char.ToLower(s[0]) + s.Substring(1);
            }

            return s;
        }

        public static string ToPascalCase(this string s)
        {
            if (s?.Length > 0)
            {
                return char.ToUpper(s[0]) + s.Substring(1);
            }

            return s;
        }
    }
}
