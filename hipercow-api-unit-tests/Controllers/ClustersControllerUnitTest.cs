// Copyright (c) Imperial College London. All rights reserved.

namespace Hipercow_api_unit_tests.Controllers
{
    using Hipercow_api.Controllers;
    using Hipercow_api.Models;
    using Hipercow_api.Tools;
    using Microsoft.AspNetCore.Mvc;

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
            ClustersController cc = new ClustersController();
            List<string> clusters = cc.Get();
            Assert.Single(clusters);
            Assert.Equal("wpia-hn", clusters.First());
        }

        /// <summary>
        /// Test that /clusters/wrong handles incorrect
        /// cluster name tidily.
        /// </summary>
        [Fact]
        public void GetWrongCluster_ReturnsNotFound()
        {
            ClustersController cc = new ClustersController();
            IActionResult res = cc.Get("turnip");
            Assert.Equivalent(cc.NotFound(), res);
        }

        /// <summary>
        /// Test just the endpoint handling of /clusters/cluster
        /// by providing fake data.
        /// </summary>
        [Fact]
        public void GetClusterinfo_Works()
        {
            ClustersController cc = new ClustersController();
            FakeClusterInfoQuery fake = new FakeClusterInfoQuery();
            cc.MockClusterInfoQuery(fake);
            IActionResult res = cc.Get("wpia-hn");
            ClusterInfo? fakeinfo = fake.GetClusterInfo("wpia-hn");
            Assert.Equivalent(cc.Ok(fakeinfo), res);
        }

        private class FakeClusterInfoQuery : IClusterInfoQuery
        {
            public ClusterInfo? GetClusterInfo(string cluster)
            {
                return new ClusterInfo(
                    "potato",
                    64,
                    4,
                    new List<string> { "A", "B" },
                    new List<string> { "Q1", "Q2" },
                    "Q1");
            }
        }
    }
}