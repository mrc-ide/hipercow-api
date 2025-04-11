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
            var cc = new ClustersController(new ClusterInfoQuery(), new ClusterHandleCache(), new Utils());
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
            ClustersController cc = new ClustersController(new ClusterInfoQuery(), new ClusterHandleCache(), new Utils());
            Assert.Equivalent(cc.NotFound(), cc.Get("potato"));
        }

        /// <summary>
        /// Test just the endpoint handling of /clusters/cluster
        /// by providing fake data.
        /// </summary>
        [Fact]
        public void GetClusterinfo_Works()
        {
            StoreProperty[] sp1 =
            [
                new StoreProperty(NodePropertyIds.Name, "node-1"),
                new StoreProperty(NodePropertyIds.MemorySize, 32 * 1024),
                new StoreProperty(NodePropertyIds.NumCores, 4),
            ];

            StoreProperty[] sp2 =
            [
                new StoreProperty(NodePropertyIds.Name, "node-2"),
                new StoreProperty(NodePropertyIds.MemorySize, 16 * 1024),
                new StoreProperty(NodePropertyIds.NumCores, 8),
            ];

            PropertyRow[] rows =
            [
                new PropertyRow(sp1),
                new PropertyRow(sp2),
            ];

            var prs = new PropertyRowSet(null, rows);

            var mockScheduler = new Mock<IScheduler>();
            mockScheduler.Setup(x => x.Connect("potato")).Verifiable();

            var mockHandleCache = new Mock<IClusterHandleCache>();
            mockHandleCache.Setup(x => x.GetClusterHandle("potato")).Returns(mockScheduler.Object);

            Mock<IUtils> mockUtils = new();
            mockUtils.Setup(x => x.NodesQuery(
               It.IsAny<IScheduler>(),
               It.IsAny<IPropertyIdCollection>(),
               It.IsAny<IFilterCollection>(),
               It.IsAny<ISortCollection>())).Returns(prs);

            var cc = new ClustersController(new ClusterInfoQuery(), mockHandleCache.Object, mockUtils.Object);
            var res = cc.Get("potato");

            ClusterInfo expected = new ClusterInfo("potato", 32, 8, ["node-1", "node-2"], [], string.Empty);
            Assert.Equivalent(cc.Ok(expected), cc.Get("potato"));
        }
    }
}