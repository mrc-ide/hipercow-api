// Copyright (c) Imperial College London. All rights reserved.

namespace HipercowApiUnitTests.Controllers
{
    using HipercowApi.Controllers;
    using HipercowApi.Models;
    using HipercowApi.Tools;
    using Moq;

    /// <summary>
    /// Test the /clusters endpoint.
    /// </summary>
    public class ClusterLoadControllerUnitTest
    {
        /// <summary>
        /// Test just the endpoint handling of /clusterload/cluster
        /// by providing fake data.
        /// </summary>
        [Fact]
        public void GetClusterLoad_Works()
        {
            var potato = new ClusterLoad(
                    "potato",
                    [new NodeLoad("node1", 8, 32, "Online"),
                     new NodeLoad("node2", 0, 32, "Offline")]);

            Mock<IClusterLoadQuery> mockClusterLoadQuery = new();
            mockClusterLoadQuery.Setup(x => x.GetClusterLoad("potato", null)).Returns(potato);
            var cc = new ClusterLoadController(mockClusterLoadQuery.Object);
            Assert.Equivalent(cc.Ok(potato), cc.Get("potato"));
        }

        /// <summary>
        /// Test that /clusterload/wrong handles incorrect
        /// cluster name tidily.
        /// </summary>
        [Fact]
        public void GetWrongCluster_ReturnsNotFound()
        {
            Mock<IClusterLoadQuery> mockClusterLoadQuery = new();
            mockClusterLoadQuery.Setup(x => x.GetClusterLoad("wrong", null)).Returns((ClusterLoad?)null);
            var cc = new ClusterLoadController(mockClusterLoadQuery.Object);
            Assert.Equivalent(cc.NotFound(), cc.Get("wrong"));
        }
    }
}