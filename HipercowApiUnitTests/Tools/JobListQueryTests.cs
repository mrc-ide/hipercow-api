// Copyright (c) Imperial College London. All rights reserved.

namespace HipercowApiUnitTests.Tools
{
    using HipercowApi.Models;
    using HipercowApi.Tools;
    using Microsoft.Hpc.Scheduler;
    using Microsoft.Hpc.Scheduler.Properties;
    using Moq;

    /// <summary>
    /// Tests on the unit class.
    /// </summary>
    public class JobListQueryTests
    {
        /// <summary>
        /// Job List Query returns data - no filters.
        /// </summary>
        [Fact]
        public void JobListQueryNoFilters_Works()
        {
            var mockRowEnum = new Mock<ISchedulerRowEnumerator>();
            mockRowEnum.Setup(x => x.GetRows(It.IsAny<int>())).Returns(new PropertyRowSet(null, AllJobs()));
            mockRowEnum.Setup(x => x.GetRows(1)).Returns(new PropertyRowSet(null, OneJob()));
            var mockScheduler = new Mock<IScheduler>();
            mockScheduler.Setup(x => x.OpenJobEnumerator(
                It.IsAny<PropertyIdCollection>(),
                It.IsAny<IFilterCollection>(),
                It.IsAny<ISortCollection>())).Returns(mockRowEnum.Object);

            JobListQuery jlq = new();
            JobList jl = jlq.GetJobList("wpia-hn", mockScheduler.Object, null, null, int.MaxValue);
            Assert.Equal(5, jl.Jobs.Count);

            jl = jlq.GetJobList("wpia-hn", mockScheduler.Object, null, null, 1);
            Assert.Single(jl.Jobs);
        }

        /// <summary>
        /// Job List Query returns data - no filters.
        /// </summary>
        [Fact]
        public void JobListQueryFilters_Works()
        {
            var mockRowEnum = new Mock<ISchedulerRowEnumerator>();
            mockRowEnum.Setup(x => x.GetRows(It.IsAny<int>())).Returns(new PropertyRowSet(null, BobsJobs()));
            var mockScheduler = new Mock<IScheduler>();
            mockScheduler.Setup(x => x.OpenJobEnumerator(
                It.IsAny<PropertyIdCollection>(),
                It.IsNotNull<IFilterCollection>(),
                It.IsAny<ISortCollection>())).Returns(mockRowEnum.Object);
            var mockFilter = new Mock<IFilterCollection>();
            mockFilter.Setup(x => x.Add(
                It.IsAny<FilterOperator>(),
                It.IsAny<PropId>(),
                It.IsAny<object>())).Verifiable();
            mockScheduler.Setup(x => x.CreateFilterCollection()).Returns(mockFilter.Object);

            JobListQuery jlq = new();
            JobList jl = jlq.GetJobList("wpia-hn", mockScheduler.Object, "Bob", null, int.MaxValue);
            Assert.Equal(3, jl.Jobs.Count);
            mockFilter.Verify(
                x => x.Add(
                    It.IsAny<FilterOperator>(),
                    It.IsAny<PropId>(),
                    It.IsAny<object>()),
                Times.Once());
        }

        private static PropertyRow FakeJobInfo(
             int id,
             string user,
             string name,
             string state)
        {
            return new PropertyRow([
                    new StoreProperty(JobPropertyIds.Id, id),
                    new StoreProperty(JobPropertyIds.UserName, user),
                    new StoreProperty(JobPropertyIds.Name, name),
                    new StoreProperty(JobPropertyIds.State, state)]);
        }

        private static PropertyRow[] AllJobs()
        {
            return [
                FakeJobInfo(1000, "Alice", "Job A1", "Finished"),
                FakeJobInfo(1001, "Alice", "Job A2", "Running"),
                FakeJobInfo(1002, "Bob", "Job B1", "Queued"),
                FakeJobInfo(1003, "Bob", "Job B2", "Canceled"),
                FakeJobInfo(1004, "Bob", "Job B3", "Failed")];
        }

        private static PropertyRow[] OneJob()
        {
            return [
                FakeJobInfo(1000, "Alice", "Job A1", "Finished")];
        }

        private static PropertyRow[] BobsJobs()
        {
            return [
                FakeJobInfo(1002, "Bob", "Job B1", "Queued"),
                FakeJobInfo(1003, "Bob", "Job B2", "Canceled"),
                FakeJobInfo(1004, "Bob", "Job B3", "Failed")];
        }
    }
}