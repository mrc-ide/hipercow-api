// Copyright (c) Imperial College London. All rights reserved.

namespace Hipercow_api_unit_tests.Tools
{
    using Hipercow_api.Tools;
    using Moq;

    /// <summary>
    /// Test the /clusters/cluster info endpoint.
    /// </summary>
    public class ClusterHandleCacheTests
    {
        /// <summary>
        /// Add the same schedulers twice and verify we only
        /// have each once in the cache.
        /// </summary>
        [Fact]
        public void ClusterHandleCache_nodups()
        {
            var fakeHPC1 = Helpers.MockScheduler();
            var fakeHPC2 = Helpers.MockScheduler();
            var fakeClusters = new List<string> { "fake1", "fake2" };

            var chc = new ClusterHandleCache();
            chc.InitialiseHandles(new List<string> { "fake1" }, fakeClusters, fakeHPC1.Object);
            chc.InitialiseHandles(new List<string> { "fake2" }, fakeClusters, fakeHPC2.Object);
            chc.InitialiseHandles(new List<string> { "fake1", "fake2" });
            Assert.Equal(2, chc.Count());
        }

        /// <summary>
        /// Verify that getting not-yet-existing cluster handles creates,
        /// then later returns the same object.
        /// </summary>
        [Fact]
        public void ClusterHandleCache_sameinstance()
        {
            var fakeHPC1 = Helpers.MockScheduler();
            var fakeHPC2 = Helpers.MockScheduler();
            var fakeClusters = new List<string> { "fake1", "fake2" };
            var chc = new ClusterHandleCache();
            var fake1 = chc.GetClusterHandle("fake1", fakeClusters, fakeHPC1.Object);
            var fake2 = chc.GetClusterHandle("fake2", fakeClusters, fakeHPC2.Object);
            var fake1b = chc.GetClusterHandle("fake1");
            Assert.Same(fake1, fake1b);
            Assert.NotEqual(fake1, fake2);
        }
    }
}