﻿// Copyright (c) Imperial College London. All rights reserved.

namespace Hipercow_api_unit_tests.Tools
{
    using Hipercow_api.Models;
    using Hipercow_api.Tools;
    using Microsoft.ComputeCluster;
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
            {
                new StoreProperty(NodePropertyIds.Name, "node-1"),
                new StoreProperty(NodePropertyIds.MemorySize, 32 * 1024),
                new StoreProperty(NodePropertyIds.NumCores, 4),
            };

            StoreProperty[] sp2 =
            {
                new StoreProperty(NodePropertyIds.Name, "node-2"),
                new StoreProperty(NodePropertyIds.MemorySize, 16 * 1024),
                new StoreProperty(NodePropertyIds.NumCores, 8),
            };

            PropertyRow[] rows =
            [
                new PropertyRow(sp1),
                new PropertyRow(sp2),
            ];

            PropertyRowSet prs = new PropertyRowSet(null, rows);

            // Mock the cluster headnode - make the Connect and NodesQuery wrappers do nothing.
            Mock<IHipercowScheduler> fakeHPC = new();
            fakeHPC.Setup(x => x.Connect(It.IsAny<string>()));
            fakeHPC.Setup(x => x.NodesQuery(It.IsAny<IPropertyIdCollection>(), It.IsAny<IFilterCollection>(), It.IsAny<ISortCollection>())).Returns(prs).Verifiable();

            // Test that we can get back the fake data with an info query.
            ClusterInfoQuery q = new ClusterInfoQuery();
            ClusterInfo? info = q.GetClusterInfo("wpia-hn", fakeHPC.Object);
            Assert.NotNull(info);
            Assert.Equal(32, info.MaxRam);
            Assert.Equal(8, info.MaxCores);

            Assert.Equal("wpia-hn", info.Name);
            Assert.Equal("AllNodes", info.DefaultQueue);
            Assert.Equivalent(new List<string> { "node-1", "node-2" }, info.Nodes);
            Assert.Equivalent(new List<string> { "AllNodes", "Training" }, info.Queues);

            // Test that an invalid scheduler returns null
            Assert.Null(q.GetClusterInfo("potato"));
        }
    }
}