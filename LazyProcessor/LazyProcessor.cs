namespace LazyProcessor
{
    using System;
    using System.Collections.Generic;
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

            var enumerator = sourceValues.GetEnumerator();
            var result = new LazyResult<TResult>();
            for (var i = 0; i < maxDegreeOfParallelism; i++)
            {
                var initialData = enumerator.GetNextBatch(batchSize);
                if (initialData.Length == 0)
                    break;

                var thread = new Thread(() =>
                {
                    while (initialData.Length > 0)
                    {
                        result.AddRange(getBatchResultFunc(initialData));
                        initialData = enumerator.GetNextBatch(batchSize);
                    }

                    result.RemoveWorker();
                });
                result.AddWorker();
                thread.Start();
            }
            
            return result;
        }
    }
}
