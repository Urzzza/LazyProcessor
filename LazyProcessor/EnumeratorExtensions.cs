using System.Collections.Generic;

namespace LazyProcessor
{
    public static class EnumeratorExtensions
    {
        public static T[] GetNextBatch<T>(this IEnumerator<T> enumerator, int count)
        {
            lock (enumerator)
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