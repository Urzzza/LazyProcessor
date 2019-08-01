using System.Threading;

namespace LazyProcessor
{
    public class SynchronizationContext
    {
        public ManualResetEvent DataEvent = new ManualResetEvent(false);

        public bool ProcessingFinished = false;
    }
}