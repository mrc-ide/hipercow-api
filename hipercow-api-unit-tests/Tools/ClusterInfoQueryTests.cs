// Copyright (c) Imperial College London. All rights reserved.

namespace Hipercow_api_unit_tests.Tools
{
    using Hipercow_api.Models;
    using Hipercow_api.Tools;
    using Microsoft.Hpc.Scheduler.Properties;

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
            ClusterInfoQuery q = new ClusterInfoQuery();
            HipercowScheduler fake = new HipercowScheduler(true);

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

            fake.SetTestData(new PropertyRowSet(null, rows));
            ClusterHandleCache chc = ClusterHandle.GetClusterHandleCache();
            chc.Remove("wpia-hn");
            chc.Add("wpia-hn", fake);
            ClusterInfo? info = q.GetClusterInfo("wpia-hn");
            Assert.NotNull(info);
            Assert.Equal(32, info.MaxRam);
            Assert.Equal(8, info.MaxCores);

            chc.Remove("wpia-hn");
            ClusterHandleCache.InitialiseHandles(new List<string> { "fake1", "fake2", "fake1" }, true);
            Assert.Equal(2, chc.Count());

            chc.Remove("fake1");
            chc.Remove("fake2");
            HipercowScheduler? fake1 = ClusterHandle.GetClusterHandle("fake1", true);
            HipercowScheduler? fake2 = ClusterHandle.GetClusterHandle("fake2", true);
            HipercowScheduler? fake1b = ClusterHandle.GetClusterHandle("fake1", true);
            Assert.Equal(fake1, fake1b);
            Assert.NotEqual(fake1, fake2);
        }
    }
}