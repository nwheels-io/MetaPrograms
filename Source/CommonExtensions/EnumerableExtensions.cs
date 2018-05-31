using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonExtensions
{
    public static class EnumerableExtensions
    {
        public static IList<T> FindCommonPrefix<T>(this IEnumerable<IEnumerable<T>> sequences, IEqualityComparer<T> comparer = null)
        {
            var effectiveComparer = comparer ?? EqualityComparer<T>.Default;
            List<T> prefix = null;

            foreach (var singleSequence in sequences)
            {
                if (prefix == null)
                {
                    prefix = singleSequence.ToList();
                }
                else
                {
                    var localPrefix = prefix; // avoid access to modified closure
                    var matchedCount = singleSequence
                        .TakeWhile((item, index) => index < localPrefix.Count && effectiveComparer.Equals(item, localPrefix[index]))
                        .Count();

                    if (matchedCount < prefix.Count)
                    {
                        prefix.RemoveRange(matchedCount, prefix.Count - matchedCount);
                    }
                }
            }

            return prefix;
        }
    }
}
