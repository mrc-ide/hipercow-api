// Copyright (c) Imperial College London. All rights reserved.

namespace HipercowApiUnitTests.Tools
{
    using HipercowApi.Controllers;
    using HipercowApi.Models;
    using HipercowApi.Tools;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Hpc.Scheduler.Properties;
    using Microsoft.Hpc.Scheduler.Session;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;

    /// <summary>
    /// Tests on the unit class.
    /// </summary>
    public class UtilsTests
    {
        /// <summary>
        /// HPCString extracts a string value from a valid
        /// StoreProperty.
        /// </summary>
        [Fact]
        public void HPCString_Works()
        {
            var p = new StoreProperty("test_string", "test_val");
            var s = Utils.HPCString(p);
            Assert.Equal("test_val", s);
        }

        /// <summary>
        /// HPCString returns a valid int from StoreProperty.
        /// </summary>
        [Fact]
        public void HPCStringInt_Works()
        {
            var p = new StoreProperty("test_int", 256);
            var i = Utils.HPCInt(p);
            Assert.Equal(256, i);
        }

        /// <summary>
        /// HPCJobState returns correct values.
        /// </summary>
        [Fact]
        public void HPCJobState_Works()
        {
            Assert.Equal(JobState.Canceled, Utils.HPCJobState("Canceled"));
            Assert.Equal(JobState.Failed, Utils.HPCJobState("Failed"));
            Assert.Equal(JobState.Finished, Utils.HPCJobState("Finished"));
            Assert.Equal(JobState.Queued, Utils.HPCJobState("Queued"));
            Assert.Equal(JobState.Running, Utils.HPCJobState("Running"));
            Assert.Null(Utils.HPCJobState("Potato"));
        }
    }
}
