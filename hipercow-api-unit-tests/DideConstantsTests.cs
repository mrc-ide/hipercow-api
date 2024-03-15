using Hipercow_api;

namespace hipercow_api_unit_tests
{
    public class DideConstantsTest
    {
        [Fact]
        public void GetDideClusters_works()
        {
            List<string> res = DideConstants.GetDideClusters();
            Assert.Single(res);
            Assert.Contains("wpia-hn", res);
        }

        [Fact]
        public void GetQueues_works()
        {
            List<string> res = DideConstants.GetQueues("wpia-hn");
            Assert.Equal(2, res.Count);
            Assert.Contains("AllNodes", res);
            Assert.Contains("Training", res);
            Assert.Empty(DideConstants.GetQueues("turnip"));
        }

        [Fact]
        public void GetDefaultQueue_works()
        {
            Assert.Equal("AllNodes", DideConstants.GetDefaultQueue("wpia-hn"));
            Assert.Empty(DideConstants.GetDefaultQueue("turnip"));
        }
    }
}