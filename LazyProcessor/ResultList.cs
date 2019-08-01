using System.Collections;
using System.Collections.Generic;

namespace LazyProcessor
{
    public class ResultList<T> : IEnumerable<T>
    {
        private readonly SynchronizationContext context;
        private readonly List<T> data;
        private readonly IEnumerator<T> enumerator;
        private readonly object _lock = new object(); 
        
        public ResultList(SynchronizationContext context)
        {
            this.context = context;
            data = new List<T>();
            enumerator = new ConcurrentEnumerator<T>(data.GetEnumerator(), context);
        }

        public void AddRange(IEnumerable<T> newData)
        {
            lock (_lock)
            {
                data.AddRange(newData);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return enumerator;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
