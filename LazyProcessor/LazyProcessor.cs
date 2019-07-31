namespace LazyProcessor
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    public class LazyProcessor
    {
        public IEnumerable<TResult> ProcessInBatches<TValue, TResult>(
            IEnumerable<TValue> sourceValues,
            Func<TValue[], TResult[]> getBatchResultFunc,
            int batchSize = 100,
            int maxDegreeOfParallelism = 10)
        {
            if (maxDegreeOfParallelism <= 0)
                throw new ArgumentException($"{nameof(maxDegreeOfParallelism)} must be > 0");

            var queue = new ConcurrentQueue<(int index, IEnumerable<TValue> data)>(sourceValues.Chunk(batchSize));
            var result = new ConcurrentBag<(int index, IEnumerable<TResult> data)>();
            var threads = new List<Thread>(maxDegreeOfParallelism);
            for (var i = 0; i < maxDegreeOfParallelism; i++)
            {
                var thread = new Thread(() =>
                {
                    while (queue.TryDequeue(out var source))
                    {
                        result.Add((source.index, getBatchResultFunc(source.data.ToArray())));
                    }
                });
                thread.Start();
                threads.Add(thread);
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }

            return result.OrderBy(x => x.index).SelectMany(x => x.data);
        }
    }
}
