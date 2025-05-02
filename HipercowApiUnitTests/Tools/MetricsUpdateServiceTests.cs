// Copyright (c) Imperial College London. All rights reserved.

namespace HipercowApiUnitTests.Tools
{
    using System.Diagnostics.CodeAnalysis;
    using HipercowApi.Tools;
    using Microsoft.Hpc.Scheduler;
    using Microsoft.Hpc.Scheduler.Properties;
    using Moq;

    /// <summary>
    /// Tests on the unit class.
    /// </summary>
    public class MetricsUpdateServiceTests
    {
        /// <summary>
        /// GetCoreHours works for a multi-core single node job.
        /// </summary>
        [Fact]
        public void GetCoreHoursCores_Works()
        {
            DateTime end = new(2025, 5, 1, 12, 0, 0);
            DateTime start = end.AddHours(-1);
            PropertyRow p = new([
                    new StoreProperty(JobPropertyIds.StartTime, start),
                    new StoreProperty(JobPropertyIds.EndTime, end),
                    new StoreProperty(JobPropertyIds.UnitType, JobUnitType.Core),
                    new StoreProperty(JobPropertyIds.MinCores, 2)]);

            Assert.Equal(2, MetricsUpdateService.GetCoreHours(p, JobState.Finished));
        }

        /// <summary>
        /// GetCoreHours works for a multi-node node job. (Assuming 32-cores per node).
        /// </summary>
        [Fact]
        public void GetCoreHoursNodes_Works()
        {
            DateTime end = new(2025, 5, 1, 12, 0, 0);
            DateTime start = end.AddHours(-1);
            PropertyRow p = new([
                    new StoreProperty(JobPropertyIds.StartTime, start),
                    new StoreProperty(JobPropertyIds.EndTime, end),
                    new StoreProperty(JobPropertyIds.UnitType, JobUnitType.Node),
                    new StoreProperty(JobPropertyIds.MinNodes, 3)]);

            Assert.Equal(96, MetricsUpdateService.GetCoreHours(p, JobState.Finished));
        }

        /// <summary>
        /// GetCoreHours works for a still-running job.
        /// </summary>
        [Fact]
        public void GetCoreHoursRunning_Works()
        {
            DateTime start = DateTime.Now.AddHours(-1.5);
            PropertyRow p = new([
                    new StoreProperty(JobPropertyIds.StartTime, start),
                    new StoreProperty(JobPropertyIds.UnitType, JobUnitType.Core),
                    new StoreProperty(JobPropertyIds.MinCores, 5)]);
            float corehours = MetricsUpdateService.GetCoreHours(p, JobState.Running);
            Assert.True(corehours > 1.4 * 5);
            Assert.True(corehours < 1.6 * 5);
        }

        /// <summary>
        /// Test metrics update call. I get strange partial code coverage
        /// on the Assert.Equal lines - not sure why.
        /// </summary>
        [Fact]
        [ExcludeFromCodeCoverage]
        public void MetricsUpdate_Works()
        {
            var mockScheduler = new Mock<IScheduler>();
            mockScheduler.Setup(x => x.Connect("potato")).Verifiable();
            var mockRowEnumerator = new Mock<ISchedulerRowEnumerator>();
            mockScheduler.Setup(x => x.OpenJobEnumerator(
                It.IsAny<PropertyIdCollection>(),
                It.IsAny<IFilterCollection>(),
                It.IsAny<SortCollection>())).Returns(mockRowEnumerator.Object);
            mockScheduler.Setup(x => x.CreateFilterCollection()).
                Returns(new FilterCollection());
            mockRowEnumerator.Setup(x => x.GetRows(It.IsAny<int>())).
                Returns(new PropertyRowSet(null, AllJobs()));

            var mockCHC = new Mock<IClusterHandleCache>();
            mockCHC.Setup(x => x.GetClusterHandle(It.IsAny<string>())).
                Returns(mockScheduler.Object);
            MetricsUpdateService mus = new(mockCHC.Object);

            mus.UpdateByState("potato", JobState.Finished, 24);
            var res = mus.GetUserJobs();

            Assert.Equal(1, res["A"]["Finished"]);
            Assert.Equal(2, res["A"]["coreHours"]);
            Assert.Equal(1, res["B"]["Finished"]);
            Assert.Equal(2, res["B"]["coreHours"]);
            Assert.Equal(1, res["C"]["Finished"]);
            Assert.Equal(64, res["C"]["coreHours"]);
            Assert.False(res.ContainsKey("D"));

            mus.UpdateByState("potato", JobState.Canceled, 24);
            res = mus.GetUserJobs();
            Assert.False(res["A"].ContainsKey("Canceled"));
            Assert.True(res["A"].ContainsKey("Cancelled"));

            mus.UpdateByState("potato", JobState.Running, 24);
            Thread.Sleep(10);
            res = mus.GetUserJobs();
            float corehours = res["A"]["coreHours"];
            Assert.True(corehours > 6);
        }

        private static PropertyRow FakeJobInfo(
             string user,
             string owner,
             DateTime changeTime,
             DateTime startTime,
             DateTime endTime,
             int minCores,
             int minNodes,
             JobUnitType unitType)
        {
            return new PropertyRow([
                    new StoreProperty(JobPropertyIds.UserName, user),
                    new StoreProperty(JobPropertyIds.Owner, owner),
                    new StoreProperty(JobPropertyIds.ChangeTime, changeTime),
                    new StoreProperty(JobPropertyIds.StartTime, startTime),
                    new StoreProperty(JobPropertyIds.EndTime, endTime),
                    new StoreProperty(JobPropertyIds.MinCores, minCores),
                    new StoreProperty(JobPropertyIds.MinNodes, minNodes),
                    new StoreProperty(JobPropertyIds.UnitType, unitType)]);
        }

        private static PropertyRow[] AllJobs()
        {
            DateTime t1 = DateTime.Now.AddHours(-2);
            DateTime t2 = DateTime.Now.AddHours(-1);
            DateTime dtnew = DateTime.Now.AddHours(-0.5);
            DateTime dtold = DateTime.Now.AddDays(-30);

            return [
                FakeJobInfo("A", "A", dtnew, t1, t2, 2, 0, JobUnitType.Core),
                FakeJobInfo(string.Empty, "B", dtnew, t1, t2, 2, 0, JobUnitType.Core),
                FakeJobInfo("C", "C", dtnew, t1, t2, 0, 2, JobUnitType.Node),
                FakeJobInfo("D", "D", dtold, t1, t2, 0, 2, JobUnitType.Node)
            ];
        }
    }
}
