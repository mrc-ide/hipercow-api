// Copyright (c) Imperial College London. All rights reserved.

namespace Hipercow_api_unit_tests.Controllers
{
    using Hipercow_api.Controllers;
    using Hipercow_api.Models;
    using Hipercow_api.Tools;
    using Microsoft.AspNetCore.Mvc;
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
            ClustersController cc = new ClustersController(new ClusterInfoQuery());
            List<string> clusters = cc.Get();
            Assert.Equal(new List<string> { "wpia-hn" }, clusters);
        }

        /// <summary>
        /// Test that /clusters/wrong handles incorrect
        /// cluster name tidily.
        /// </summary>
        [Fact]
        public void GetWrongCluster_ReturnsNotFound()
        {
            Mock<IClusterInfoQuery> mockClusterInfoQuery = new();
            mockClusterInfoQuery.Setup(x => x.GetClusterInfo("wrong")).Returns((ClusterInfo?)null);
            ClustersController cc = new ClustersController(mockClusterInfoQuery.Object);
            IActionResult res = cc.Get("wrong");
            Assert.Equivalent(cc.NotFound(), res);
        }

        /// <summary>
        /// Test just the endpoint handling of /clusters/cluster
        /// by providing fake data.
        /// </summary>
        [Fact]
        public void GetClusterinfo_Works()
        {
            ClusterInfo potato = new ClusterInfo(
                    "potato",
                    64,
                    4,
                    new List<string> { "A", "B" },
                    new List<string> { "Q1", "Q2" },
                    "Q1");

            Mock<IClusterInfoQuery> mockClusterInfoQuery = new();
            mockClusterInfoQuery.Setup(x => x.GetClusterInfo("potato")).Returns(potato);
            ClustersController cc = new ClustersController(mockClusterInfoQuery.Object);
            IActionResult res = cc.Get("potato");
            Assert.Equivalent(cc.Ok(potato), res);
        }
    }
}