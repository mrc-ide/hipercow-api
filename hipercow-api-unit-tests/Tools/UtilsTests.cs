// Copyright (c) Imperial College London. All rights reserved.

namespace Hipercow_api_unit_tests.Tools
{
    using Hipercow_api.Tools;
    using Microsoft.Hpc.Scheduler.Properties;

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
    }
}