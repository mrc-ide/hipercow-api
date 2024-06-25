// Copyright (c) Imperial College London. All rights reserved.

namespace Hipercow_api_unit_tests.Tools
{
    using Hipercow_api.Models;
    using Hipercow_api.Tools;
    using Microsoft.Hpc.Scheduler;
    using Microsoft.Hpc.Scheduler.Properties;
    using Moq;

    /// <summary>
    /// Test the /clusters/cluster info endpoint.
    /// </summary>
    public class ClusterHandleCacheTests
    {
        /// <summary>
        /// Test we can cache cluster handles rather than recreating every time.
        /// </summary>
        [Fact]
        public void ClusterHandleCache_works()
        {
            Mock<IHipercowScheduler> fakeHPC1 = new();
            fakeHPC1.Setup(x => x.Connect(It.IsAny<string>()));

            Mock<IHipercowScheduler> fakeHPC2 = new();
            fakeHPC2.Setup(x => x.Connect(It.IsAny<string>()));

            List<string> fakeClusters = new List<string> { "fake1", "fake2" };
            ClusterHandleCache chc = new ClusterHandleCache();
            chc.InitialiseHandles(new List<string> { "fake1" }, fakeClusters, fakeHPC1.Object);
            chc.InitialiseHandles(new List<string> { "fake2" }, fakeClusters, fakeHPC2.Object);
            chc.InitialiseHandles(new List<string> { "fake1", "fake2" });
            Assert.Equal(2, chc.Count());

            chc.Remove("fake1");
            chc.Remove("fake2");
            IHipercowScheduler? fake1 = chc.GetClusterHandle("fake1", fakeClusters, fakeHPC1.Object);
            IHipercowScheduler? fake2 = chc.GetClusterHandle("fake2", fakeClusters, fakeHPC2.Object);
            IHipercowScheduler? fake1b = chc.GetClusterHandle("fake1");
            Assert.Equal(fake1, fake1b);
            Assert.NotEqual(fake1, fake2);
        }
    }
}