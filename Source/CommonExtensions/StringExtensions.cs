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

        public static string TrimEndStartingWith(this string s, string subString)
        {
            int position;
            
            if (s != null && (position = s.IndexOf(subString)) >= 0)
            {
                return s.Substring(0, position);
            }

            return s;
        }
    }
}
