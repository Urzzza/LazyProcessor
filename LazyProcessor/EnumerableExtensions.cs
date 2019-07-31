namespace LazyProcessor
{
    using System;
    using System.Collections.Generic;

    public static class EnumerableExtensions
    {
        public static IEnumerable<(int index, IEnumerable<T> data)> Chunk<T>(this IEnumerable<T> source, int chunkSize)
        {
            if (chunkSize <= 0)
                throw new ArgumentException($"{nameof(chunkSize)} must be > 0");

            using (var enumerator = source.GetEnumerator())
            {
                var index = 0;
                var result = new List<T>();
                while (enumerator.MoveNext())
                {
                    result.Add(enumerator.Current);

                    if (result.Count == chunkSize)
                    {
                        yield return (index++, result);
                        result = new List<T>();
                    }
                }

                if (result.Count > 0)
                    yield return (index, result);
            }
        }
    }
}