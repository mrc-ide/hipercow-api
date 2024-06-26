// Copyright (c) Imperial College London. All rights reserved.

namespace Hipercow_api_unit_tests.Tools
{
    using Hipercow_api.Tools;

    /// <summary>
    /// Tests for the DideConstants class.
    /// </summary>
    public class DideConstantsTests
    {
        /// <summary>
        /// Test that GetDideClusters returns
        /// the hardcoded value.
        /// </summary>
        [Fact]
        public void GetDideClusters_works()
        {
            var res = DideConstants.GetDideClusters();
            Assert.Single(res);
            Assert.Contains("wpia-hn", res);
        }

        /// <summary>
        /// Test that we can get the list of published queues.
        /// </summary>
        [Fact]
        public void GetQueues_works()
        {
            var res = DideConstants.GetQueues("wpia-hn");
            Assert.Equal(2, res.Count);
            Assert.Contains("AllNodes", res);
            Assert.Contains("Training", res);
            Assert.Empty(DideConstants.GetQueues("turnip"));
        }

        /// <summary>
        /// Test that we can get the default queue.
        /// </summary>
        [Fact]
        public void GetDefaultQueue_works()
        {
            Assert.Equal("AllNodes", DideConstants.GetDefaultQueue("wpia-hn"));
            Assert.Empty(DideConstants.GetDefaultQueue("turnip"));
        }
    }
}