// Copyright (c) Imperial College London. All rights reserved.

namespace HipercowApiUnitTests.Tools
{
    using HipercowApi.Tools;
    using Microsoft.Hpc.Scheduler;
    using Microsoft.Hpc.Scheduler.Properties;
    using Moq;

    /// <summary>
    /// Test the /clusters/cluster info endpoint.
    /// </summary>
    public class ClusterInfoQueryTests
    {
        /// <summary>
        /// Test the endpoint with fake data.
        /// </summary>
        [Fact]
        public void GetClusterInfo_works()
        {
            // Below is fake data in the form that MS HPC
            // might return if we asked it.
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

            // Mock the cluster headnode - make Connect do nothing, and NodesQuery always return our data
            var fakeHPC = Helpers.MockScheduler();
            fakeHPC.Setup(x => x.NodesQuery(It.IsAny<IPropertyIdCollection>(), It.IsAny<IFilterCollection>(), It.IsAny<ISortCollection>())).Returns(prs).Verifiable();

            // Test that we can get back the fake data with an info query.
            var info = new ClusterInfoQuery().GetClusterInfo("wpia-hn", fakeHPC.Object);

            Assert.NotNull(info);
            Assert.Equal(32, info.MaxRam);
            Assert.Equal(8, info.MaxCores);
            Assert.Equal("wpia-hn", info.Name);
            Assert.Equal("AllNodes", info.DefaultQueue);
            Assert.Equivalent(new List<string> { "node-1", "node-2" }, info.Nodes);
            Assert.Equivalent(new List<string> { "AllNodes", "Training" }, info.Queues);
        }

        /// <summary>
        /// Test that info for an invalid scheduler returns null.
        /// </summary>
        [Fact]
        public void GetClusterInfo_Invalid_works()
        {
            Assert.Null(new ClusterInfoQuery().GetClusterInfo("potato"));
        }
    }
}