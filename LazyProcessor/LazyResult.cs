﻿using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace LazyProcessor
{
    public class LazyResult<T> : IEnumerable<T>, IEnumerator<T>
    {
        private readonly object _lock = new object();
        public readonly int Capacity;
        private readonly ConcurrentQueue<T> ResultQueue = new ConcurrentQueue<T>();
        public readonly ManualResetEvent DataEvent = new ManualResetEvent(false);
        public readonly ManualResetEvent CapacityEvent = new ManualResetEvent(true);
        private int CurrentWorkersCount;
        private T Value;

        private bool ProcessingFinished => CurrentWorkersCount == 0;

        public LazyResult(int capacity)
        {
            Capacity = capacity;
        }

        public IEnumerator<T> GetEnumerator() => this;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool MoveNext()
        {
            while (ResultQueue.Count > 0 || !ProcessingFinished)
            {
                DataEvent.WaitOne(TimeSpan.FromMilliseconds(400));
                lock (_lock)
                {
                    if (ResultQueue.TryDequeue(out Value))
                    {
                        CapacityEvent.Set();
                        return true;
                    }

                    DataEvent.Reset();
                }
            }

            return false;
        }

        public void IncreaseWorkerCount() => Interlocked.Increment(ref CurrentWorkersCount);

        public void DecreaseWorkerCount() => Interlocked.Decrement(ref CurrentWorkersCount);
        
        public void AddRange(IEnumerable<T> newData)
        {
            var enumerator = newData.GetEnumerator();
            while (true)
            {
                if (ResultQueue.Count >= Capacity)
                {
                    CapacityEvent.Reset();
                    CapacityEvent.WaitOne();
                    continue;
                }

                if (!enumerator.MoveNext())
                    break;

                ResultQueue.Enqueue(enumerator.Current);
                DataEvent.Set();
            }
        }

        public void Reset() {}

        public T Current
        {
            get
            {
                lock (_lock)
                {
                    return Value;
                }
            }
        }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
        }
    }
}