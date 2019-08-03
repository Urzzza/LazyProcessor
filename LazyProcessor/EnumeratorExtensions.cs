using System.Collections.Generic;

namespace LazyProcessor
{
    public static class EnumeratorExtensions
    {
        private static object _lock = new object();

        public static T[] GetNextBatch<T>(this IEnumerator<T> enumerator, int count)
        {
            lock (_lock)
            {
                var result = new List<T>(count);
                var taken = 0;
                while (taken < count && enumerator.MoveNext())
                {
                    result.Add(enumerator.Current);
                    taken++;
                }

                return result.ToArray();
            }
        }
    }
}