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
            var mockCore1 = new Mock<ISchedulerCore>();
            mockCore1.Setup(x => x.State).Returns(SchedulerCoreState.Busy);
            var mockCore2 = new Mock<ISchedulerCore>();
            mockCore2.Setup(x => x.State).Returns(SchedulerCoreState.Idle);
            var mockCore3 = new Mock<ISchedulerCore>();
            mockCore3.Setup(x => x.State).Returns(SchedulerCoreState.Offline);

            SchedulerCollection<ISchedulerCore> mockCores = new SchedulerCollection<ISchedulerCore>
                { mockCore1.Object, mockCore2.Object, mockCore3.Object };

            var mockNode1 = new Mock<ISchedulerNode>();
            mockNode1.Setup(x => x.GetCores()).Returns(mockCores);
            mockNode1.Setup(x => x.Name).Returns("node-1");
            mockNode1.Setup(x => x.State).Returns(NodeState.Online);
            mockNode1.Setup(x => x.NumberOfCores).Returns(3);

            var mockCore4 = new Mock<ISchedulerCore>();
            mockCore4.Setup(x => x.State).Returns(SchedulerCoreState.Busy);
            var mockCore5 = new Mock<ISchedulerCore>();
            mockCore5.Setup(x => x.State).Returns(SchedulerCoreState.Idle);
            SchedulerCollection<ISchedulerCore> mockCores2 = new SchedulerCollection<ISchedulerCore>
                { mockCore4.Object, mockCore5.Object };

            var mockNode2 = new Mock<ISchedulerNode>();
            mockNode2.Setup(x => x.GetCores()).Returns(mockCores2);
            mockNode2.Setup(x => x.Name).Returns("node-2");
            mockNode2.Setup(x => x.State).Returns(NodeState.Offline);
            mockNode2.Setup(x => x.NumberOfCores).Returns(2);

            SchedulerCollection<ISchedulerNode> mockNodeList = new SchedulerCollection<ISchedulerNode>
                { mockNode1.Object, mockNode2.Object};

            var mockScheduler = new Mock<IScheduler>();
            mockScheduler.Setup(x => x.Connect("potato")).Verifiable();
            mockScheduler.Setup(x => x.GetNodeList(It.IsAny<IFilterCollection>(), It.IsAny<ISortCollection>())).Returns(mockNodeList);

            var mockHandleCache = new Mock<IClusterHandleCache>();
            mockHandleCache.Setup(x => x.GetClusterHandle("potato")).Returns(mockScheduler.Object);

            var cc = new ClusterLoadController(new ClusterLoadQuery(), mockHandleCache.Object);

            NodeLoad nl1 = new NodeLoad("node-1", 1, 3, "Online");
            NodeLoad nl2 = new NodeLoad("node-2", 1, 2, "Offline");
            ClusterLoad cl = new ClusterLoad("potato", [nl1, nl2]);
            Assert.Equivalent(cc.Ok(cl), cc.Get("potato"));
        }

        /// <summary>
        /// Test that /clusterload/wrong handles incorrect
        /// cluster name tidily.
        /// </summary>
        [Fact]
        public void GetWrongCluster_ReturnsNotFound()
        {
            ClusterLoadController cc = new ClusterLoadController(new ClusterLoadQuery(), new ClusterHandleCache());
            Assert.Equivalent(cc.NotFound(), cc.Get("potato"));
        }
    }
}
