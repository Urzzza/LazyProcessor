namespace LazyProcessor.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using global::LazyProcessor;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class Tests
    {
        private LazyProcessor lazyProcessor = new LazyProcessor();

        [TestMethod]
        [DataRow(28)]
        [DataRow(100)]
        [DataRow(999)]
        [DataRow(10000000)]
        public void TestOk(int count)
        {
            var sourceData = Enumerable.Range(0, count);
            var sw = Stopwatch.StartNew();
            var result = lazyProcessor.ProcessInBatches(
                sourceData,
                this.ProcessFunc,
                1000,
                10);
            CollectionAssert.AreEqual(sourceData.ToList(), result.ToList());
            Console.WriteLine($"Time: {sw.Elapsed.TotalSeconds}");
        }

        [TestMethod]
        public void TestException()
        {
            Assert.ThrowsException<ArgumentException>(() => lazyProcessor.ProcessInBatches(
                new List<int>(),
                this.ProcessFunc,
                0));
            Assert.ThrowsException<ArgumentException>(() => lazyProcessor.ProcessInBatches(
                new List<int>(),
                this.ProcessFunc,
                10,
                0));
        }

        private int[] ProcessFunc(int[] data)
        {
            Thread.Sleep(10);
            return data;
        }
    }
}
