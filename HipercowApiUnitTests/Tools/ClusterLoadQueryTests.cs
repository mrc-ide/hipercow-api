// Copyright (c) Imperial College London. All rights reserved.

namespace HipercowApiUnitTests.Tools
{
    using HipercowApi.Tools;
    using Microsoft.Hpc.Scheduler;
    using Microsoft.Hpc.Scheduler.Properties;
    using Moq;
    using static Microsoft.Hpc.Scheduler.Store.PropertyVersioning;

    /// <summary>
    /// Test the /clusters/cluster info endpoint.
    /// </summary>
    public class ClusterLoadQueryTests
    {
        /// <summary>
        /// Test the endpoint with fake data.
        /// </summary>
        [Fact]
        public void GetClusterLoad_works()
        {
            SchedulerCollection<IHipercowSchedulerCore> cores = new SchedulerCollection<IHipercowSchedulerCore>();
            for (int i = 0; i < 8; i++)
            {
                if (i <= 5)
                {
                    cores.Add(new HipercowSchedulerCore(null, SchedulerCoreState.Busy));
                }
                else
                {
                    cores.Add(new HipercowSchedulerCore(null, SchedulerCoreState.Idle));
                }
            }

            SchedulerCollection<HipercowSchedulerNode> nodeCollection = new SchedulerCollection<HipercowSchedulerNode>();
            nodeCollection.Add(new HipercowSchedulerNode(null, cores, "node1", 8, NodeState.Online));

            // Mock the cluster headnode - make Connect do nothing, and NodesQuery always return our data
            var fakeHPC = Helpers.MockScheduler();
            fakeHPC.Setup(x => x.GetNodeList(It.IsAny<IFilterCollection>(), It.IsAny<ISortCollection>())).Returns(nodeCollection).Verifiable();

            // Test that we can get back the fake data with an info query.
            var load = new ClusterLoadQuery().GetClusterLoad("wpia-hn", fakeHPC.Object);

            Assert.NotNull(load);
            Assert.Equal("wpia-hn", load.Name);
            Assert.Equal("node1", load.NodeLoads[0].Name);
            Assert.Equal("Online", load.NodeLoads[0].State);
            Assert.Equal(8, load.NodeLoads[0].NodeCores);
            Assert.Equal(6, load.NodeLoads[0].CoresInUse);
        }

        /// <summary>
        /// Test that info for an invalid scheduler returns null.
        /// </summary>
        [Fact]
        public void GetClusterLoad_Invalid_works()
        {
            Assert.Null(new ClusterLoadQuery().GetClusterLoad("potato"));
        }
    }
}