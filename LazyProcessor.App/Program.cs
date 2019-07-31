namespace LazyProcessor.App
{
    using System;
    using System.Diagnostics;
    using System.Linq;

    class Program
    {
        private static LazyProcessor lazyProcessor = new LazyProcessor();

        static void Main(string[] args)
        {
            var count = 15;
            var sourceData = Enumerable.Range(0, count);
            var sw = Stopwatch.StartNew();
            var result = lazyProcessor.ProcessInBatches(
                sourceData,
                ProcessFunc,
                9,
                10);
            Console.WriteLine($"Time: {sw.Elapsed.TotalSeconds}");
            Console.WriteLine(string.Join("|", result));
            Console.ReadKey();
        }

        private static int[] ProcessFunc(int[] data)
        {
            return data;
        }
    }
}
