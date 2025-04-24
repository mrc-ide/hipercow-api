// Copyright (c) Imperial College London. All rights reserved.

namespace HipercowApiUnitTests.Controllers
{
    using HipercowApi.Controllers;
    using HipercowApi.Models;
    using HipercowApi.Tools;
    using Microsoft.Hpc.Scheduler;
    using Moq;

    /// <summary>
    /// Test the /joblist endpoint.
    /// </summary>
    public class JobListControllerUnitTest
    {
        /// <summary>
        /// Test the /joblist/ endpoint.
        /// </summary>
        [Fact]
        public void GetJobList_Works()
        {
            var fakeList = new JobList("test", new List<JobInfo>());
            var mockQuery = new Mock<IJobListQuery>();
            mockQuery.Setup(x => x.GetJobList(
                It.IsAny<string>(),
                It.IsAny<IScheduler>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>())).Returns(fakeList);

            var mockScheduler = new Mock<IScheduler>();
            var mockHandleCache = new Mock<IClusterHandleCache>();
            mockHandleCache.Setup(x => x.GetClusterHandle("potato")).Returns(mockScheduler.Object);
            mockHandleCache.Setup(x => x.GetClusterHandle("radish")).Verifiable();

            var jlc = new JobListController(mockQuery.Object, mockHandleCache.Object);
            var res = jlc.Post("potato", string.Empty, string.Empty, 0);
            Assert.Equivalent(jlc.Ok(fakeList), res);

            res = jlc.Post("radish", string.Empty, string.Empty, 0);
            Assert.Equivalent(jlc.NotFound(), res);
        }
    }
}
