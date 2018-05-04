namespace MetaPrograms.CodeModel.Imperative
{
    internal static class StringExtensions
    {
        public static string TrimSuffix(this string s, string suffix)
        {
            if (s == null)
            {
                return s;
            }

            if (s.Length > suffix.Length && s.EndsWith(suffix))
            {
                return s.Substring(0, s.Length - suffix.Length);
            }

            return s;
        }
    }
}
