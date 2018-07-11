using System;
using System.Collections.Generic;
using System.Text;

namespace CommonExtensions
{
    public static class ListExtensions
    {
        public static void ForEach<T>(this IList<T> list, Action<T, int> action)
        {
            for (int i = 0; i < list.Count; i++)
            {
                action(list[i], i);
            }
        }
    }
}
