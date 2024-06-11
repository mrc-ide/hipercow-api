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
            ClustersController cc = new ClustersController(new FakeClusterInfoQuery());
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
            FakeClusterInfoQuery fake = new FakeClusterInfoQuery();
            ClustersController cc = new ClustersController(fake);
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
            FakeClusterInfoQuery fake = new FakeClusterInfoQuery();
            ClustersController cc = new ClustersController(fake);
            IActionResult res = cc.Get("potato");
            ClusterInfo? fakeinfo = fake.GetClusterInfo("potato");
            Assert.Equivalent(cc.Ok(fakeinfo), res);
        }

        private class FakeClusterInfoQuery : IClusterInfoQuery
        {
            public ClusterInfo? GetClusterInfo(string cluster)
            {
                if (cluster == "potato")
                {
                    return new ClusterInfo(
                    "potato",
                    64,
                    4,
                    new List<string> { "A", "B" },
                    new List<string> { "Q1", "Q2" },
                    "Q1");
                }
                else
                {
                    return null;
                }
            }
        }
    }
}