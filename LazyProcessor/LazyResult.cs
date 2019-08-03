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
        private readonly ConcurrentQueue<T> ResultQueue = new ConcurrentQueue<T>();
        public readonly ManualResetEvent DataEvent = new ManualResetEvent(false);
        private bool ProcessingFinished;
        private T Value;
        
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
                        return true;
                    }

                    DataEvent.Reset();
                }
            }

            return false;
        }

        public void FinalizeResult() => ProcessingFinished = true;
        
        public void AddRange(IEnumerable<T> newData)
        {
            foreach (var item in newData)
            {
                ResultQueue.Enqueue(item);
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