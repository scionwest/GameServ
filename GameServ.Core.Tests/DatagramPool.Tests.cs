using GameServ.Core.Tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace GameServ.Core.Tests
{
    [TestClass]
    public class DatagramPoolTests
    {
        [TestMethod]
        public void TakeOne_ReturnsNewInstance()
        {
            // Arrange
            var pool = new DatagramPool(10);

            // Act
            FakeDatagram datagram = pool.TakeOne<FakeDatagram>();

            // Assert
            Assert.IsNotNull(datagram);
        }

        [TestMethod]
        public void TakeOne_ReturnsUniqueInstances()
        {
            // Arrange
            var pool = new DatagramPool(10);

            // Act
            FakeDatagram datagram1 = pool.TakeOne<FakeDatagram>();
            FakeDatagram datagram2 = pool.TakeOne<FakeDatagram>();

            // Assert
            Assert.AreNotEqual(datagram1, datagram2);
        }

        [TestMethod]
        public void TakeOne_PerformanceMeasurement_FirstTimeUse()
        {
            // Arrange
            const int _poolSize = 100000; // 100,000
            var pool = new DatagramPool(_poolSize);

            // Act
            var watch = new Stopwatch();
            watch.Start();
            for (int count = 0; count < _poolSize; count++)
            {
                pool.TakeOne<FakeDatagram>();
            }
            watch.Stop();

            Debug.WriteLine($"Total Time: {watch.Elapsed.TotalMilliseconds}ms");
            Debug.WriteLine($"Average per allocation: {watch.Elapsed.Ticks / _poolSize} ticks");
        }

        [TestMethod]
        public void TakeOne_PerformanceMeasurement_ReUse()
        {
            // Arrange
            const int _poolSize = 100000; // 100,000
            var pool = new DatagramPool(_poolSize);
            for (int count = 0; count < _poolSize; count++)
            {
                FakeDatagram datagram = pool.TakeOne<FakeDatagram>();
                pool.Release(datagram);
            }

            // Act
            var watch = new Stopwatch();
            watch.Start();
            for (int count = 0; count < _poolSize; count++)
            {
                pool.TakeOne<FakeDatagram>();
            }
            watch.Stop();

            Debug.WriteLine($"Total Time: {watch.Elapsed.TotalMilliseconds}ms");
            Debug.WriteLine($"Average per allocation: {watch.Elapsed.Ticks / _poolSize} ticks");
        }

        [TestMethod]
        public void TakeOne_WillNotStoreMoreThanPoolSize()
        {
            // Arrange
            var pool = new DatagramPool(2);
            FakeDatagram pooledDatagram1 = pool.TakeOne<FakeDatagram>();
            FakeDatagram pooledDatagram2 = pool.TakeOne<FakeDatagram>();
            FakeDatagram unpooledDatagram = pool.TakeOne<FakeDatagram>();

            // Act
            pool.Release(pooledDatagram1);
            pool.Release(pooledDatagram2);
            pool.Release(unpooledDatagram);

            FakeDatagram recycledDatagram1 = pool.TakeOne<FakeDatagram>(); // Should equal poolDatagram1
            FakeDatagram recycledDatagram2 = pool.TakeOne<FakeDatagram>(); // Should equal poolDatagram2
            FakeDatagram recycledDatagram3 = pool.TakeOne<FakeDatagram>(); // Should equal be a new instance

            // Assert
            // Because the concurrent bag doesn't guarantee the order, we have to check them all.
            Assert.IsTrue(pooledDatagram1 == recycledDatagram1
                || pooledDatagram1 == recycledDatagram2
                || pooledDatagram1 == recycledDatagram3, "The first pooled datagram did not match the first recycled.");
            Assert.IsTrue(pooledDatagram2 == recycledDatagram1
                || pooledDatagram2 == recycledDatagram2
                || pooledDatagram2 == recycledDatagram3, "The second pooled datagram did not match the second recycled.");
            Assert.IsTrue(unpooledDatagram != recycledDatagram1
                && unpooledDatagram != recycledDatagram2
                && unpooledDatagram != recycledDatagram3, "The unpooled datagram did not match the third recycled.");
        }

        /// <summary>
        /// This test asks the pool for a new datagram, which it then marks as "used".
        /// Releasing the datagram does not clean it up (it is done lazily in the event it's never re-used)
        /// so test asks for another FakeDatagram after releasing the first. 
        /// It should recycle the previous datagram back to us, clean.
        /// We verify this by comparing the references in our assertions.
        /// </summary>
        [TestMethod]
        public void Release_CleansUpElement()
        {
            // Arrange
            var pool = new DatagramPool(10);
            FakeDatagram datagram1 = pool.TakeOne<FakeDatagram>();
            datagram1.SetAsUnsafeForReuse();

            // Act
            pool.Release(datagram1);
            FakeDatagram datagram2 = pool.TakeOne<FakeDatagram>();

            // Assert
            Assert.AreEqual(datagram1, datagram2);
            Assert.IsTrue(datagram1.IsSafeForReuse);
        }
    }
}
