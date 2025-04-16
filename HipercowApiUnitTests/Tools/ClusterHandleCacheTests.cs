// Copyright (c) Imperial College London. All rights reserved.

namespace HipercowApiUnitTests.Tools
{
    using HipercowApi.Tools;
    using Microsoft.Hpc.Scheduler;
    using Moq;

    /// <summary>
    /// Tests on the unit class.
    /// </summary>
    public class ClusterHandleCacheTests
    {
        /// <summary>
        /// Cached scheduler object is reused.
        /// </summary>
        [Fact]
        public void CachedScheduler_Works()
        {
            var mockScheduler = new Mock<IScheduler>();
            mockScheduler.Setup(x => x.Connect("potato")).Verifiable();
            var mockSchedulerFactory = new Mock<ISchedulerFactory>();
            mockSchedulerFactory.Setup(x => x.NewScheduler()).Returns(mockScheduler.Object);

            ClusterHandleCache chc = new ClusterHandleCache(mockSchedulerFactory.Object);
            IScheduler? sch1 = chc.GetClusterHandle("wpia-hn");
            IScheduler? sch2 = chc.GetClusterHandle("wpia-hn");
            Assert.Equal(sch1, mockScheduler.Object);
            Assert.Equal(sch1, sch2);
        }
    }
}