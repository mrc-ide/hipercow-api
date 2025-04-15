// Copyright (c) Imperial College London. All rights reserved.

namespace HipercowApiUnitTests.Controllers
{
    using HipercowApi.Controllers;
    using HipercowApi.Models;
    using HipercowApi.Tools;
    using Microsoft.Hpc.Scheduler;
    using Microsoft.Hpc.Scheduler.Properties;
    using Moq;

    /// <summary>
    /// Test the /clusters endpoint.
    /// </summary>
    public class ClustersControllerUnitTest
    {
        /// <summary>
        /// Test that /clusters tells us about wpia-hn.
        /// </summary>
        [Fact]
        public void GetClusterCall_Works()
        {
            var cc = new ClustersController(new ClusterInfoQuery(), new ClusterHandleCache());
            var clusters = cc.Get();
            Assert.Equal(["wpia-hn"], clusters);
        }

        /// <summary>
        /// Test that /clusters/wrong handles incorrect
        /// cluster name tidily.
        /// </summary>
        [Fact]
        public void GetWrongCluster_ReturnsNotFound()
        {
            ClustersController cc = new(new ClusterInfoQuery(), new ClusterHandleCache());
            Assert.Equivalent(cc.NotFound(), cc.Get("potato"));
        }

        /// <summary>
        /// Test just the endpoint handling of /clusters/cluster
        /// by providing fake data.
        /// </summary>
        [Fact]
        public void GetClusterinfo_Works()
        {
            PropertyRow[] rows = [FakeNodeInfo("node-1", 32, 4), FakeNodeInfo("node-2", 16, 8)];
            PropertyRowSet prs = new(null, rows);
            Mock<ISchedulerRowEnumerator> mockISchedulerRowEnumerator = new();
            mockISchedulerRowEnumerator.Setup(x => x.GetRows(It.IsAny<int>())).Returns(prs);

            Mock<IScheduler> mockScheduler = new();
            mockScheduler.Setup(x => x.Connect("potato")).Verifiable();
            mockScheduler.Setup(x => x.OpenNodeEnumerator(
                It.IsAny<IPropertyIdCollection>(),
                It.IsAny<IFilterCollection>(),
                It.IsAny<ISortCollection>())).Returns(mockISchedulerRowEnumerator.Object);

            Mock<IClusterHandleCache> mockHandleCache = new();
            mockHandleCache.Setup(x => x.GetClusterHandle("potato")).Returns(mockScheduler.Object);

            ClustersController cc = new(new ClusterInfoQuery(), mockHandleCache.Object);
            ClusterInfo expected = new ClusterInfo("potato", 32, 8, ["node-1", "node-2"], [], string.Empty);
            Assert.Equivalent(cc.Ok(expected), cc.Get("potato"));
        }

        private static PropertyRow FakeNodeInfo(string name, int ram_gb, int cores)
        {
            return new PropertyRow([new StoreProperty(NodePropertyIds.Name, name),
                    new StoreProperty(NodePropertyIds.MemorySize, ram_gb * 1024),
                    new StoreProperty(NodePropertyIds.NumCores, cores)]);
        }
    }
}