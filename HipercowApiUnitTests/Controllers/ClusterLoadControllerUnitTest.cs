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
    /// Test the /clusterload endpoint.
    /// </summary>
    public class ClusterLoadControllerUnitTest
    {
        /// <summary>
        /// Test the /clusterload/cluster
        /// by providing fake data.
        /// </summary>
        [Fact]
        public void GetClusterLoad_Works()
        {
            SchedulerCollection<ISchedulerNode> mockNodeList =
            [
                FakeNode("node-1", FakeCores([SchedulerCoreState.Busy, SchedulerCoreState.Idle, SchedulerCoreState.Offline]), NodeState.Online),
                FakeNode("node-2", FakeCores([SchedulerCoreState.Busy, SchedulerCoreState.Idle]), NodeState.Offline),
            ];

            var mockScheduler = new Mock<IScheduler>();
            mockScheduler.Setup(x => x.Connect("potato")).Verifiable();
            mockScheduler.Setup(x => x.GetNodeList(It.IsAny<IFilterCollection>(), It.IsAny<ISortCollection>())).Returns(mockNodeList);

            var mockHandleCache = new Mock<IClusterHandleCache>();
            mockHandleCache.Setup(x => x.GetClusterHandle("potato")).Returns(mockScheduler.Object);

            var cc = new ClusterLoadController(new ClusterLoadQuery(), mockHandleCache.Object);

            NodeLoad nl1 = new("node-1", 1, 3, "Online");
            NodeLoad nl2 = new("node-2", 1, 2, "Offline");
            ClusterLoad cl = new("potato", [nl1, nl2]);
            Assert.Equivalent(cc.Ok(cl), cc.Get("potato"));
        }

        /// <summary>
        /// Test that /clusterload/wrong handles incorrect
        /// cluster name tidily.
        /// </summary>
        [Fact]
        public void GetWrongCluster_ReturnsNotFound()
        {
            ClusterLoadController cc = new(new ClusterLoadQuery(), new ClusterHandleCache());
            Assert.Equivalent(cc.NotFound(), cc.Get("potato"));
        }

        private static ISchedulerCore FakeCore(SchedulerCoreState state)
        {
            var mockCore = new Mock<ISchedulerCore>();
            mockCore.Setup(x => x.State).Returns(state);
            return mockCore.Object;
        }

        private static SchedulerCollection<ISchedulerCore> FakeCores(List<SchedulerCoreState> states)
        {
            SchedulerCollection<ISchedulerCore> schedulerCores = new();
            states.ForEach(state => schedulerCores.Add(FakeCore(state)));
            return schedulerCores;
        }

        private static ISchedulerNode FakeNode(string name, SchedulerCollection<ISchedulerCore> cores, NodeState state)
        {
            var mockNode = new Mock<ISchedulerNode>();
            mockNode.Setup(x => x.GetCores()).Returns(cores);
            mockNode.Setup(x => x.Name).Returns(name);
            mockNode.Setup(x => x.State).Returns(state);
            mockNode.Setup(x => x.NumberOfCores).Returns(cores.Count);
            return mockNode.Object;
        }
    }
}
