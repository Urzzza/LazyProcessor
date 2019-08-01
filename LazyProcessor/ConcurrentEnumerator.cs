using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace LazyProcessor
{
    public class ConcurrentEnumerator<T> : IEnumerator<T>
    {
        private readonly object _lock = new object();
        private readonly IEnumerator<T> enumerator;
        private readonly SynchronizationContext context;

        public ConcurrentEnumerator(IEnumerator<T> enumerator, SynchronizationContext context)
        {
            this.enumerator = enumerator;
            this.context = context;
        }

        public bool MoveNext()
        {
            while (!context.ProcessingFinished)
            {
                context.DataEvent.WaitOne();
                lock (_lock)
                {
                    if (enumerator.MoveNext())
                    {
                        return true;
                    }

                    context.DataEvent.Reset();
                }
            }

            return false;
        }

        public void Reset()
        {
            lock (_lock)
            {
                enumerator.Reset();
            }
        }

        public T Current {
            get
            {
                lock (_lock)
                {
                    return enumerator.Current;
                }
            }
        }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            enumerator.Dispose();
        }
    }
}