namespace Hipercow_api_unit_tests
{
    using Hipercow_api;
    using Hipercow_api.Controllers;
    using Hipercow_api.Tools;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;

    public class ClustersControllerUnitTest
    {
        [Fact]
        public void GetClusterCall_Works()
        {
            ClustersController cc = new ClustersController();
            List<string> clusters = cc.Get();
            Assert.Single(clusters);
            Assert.Equal("wpia-hn", clusters.First());
        }

        [Fact]
        public void GetWrongCluster_ReturnsNotFound()
        {
            ClustersController cc = new ClustersController();
            IActionResult res = cc.Get("turnip");
            Assert.Equivalent(cc.NotFound(), res);
        }

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