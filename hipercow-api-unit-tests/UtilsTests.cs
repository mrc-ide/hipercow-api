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
            StoreProperty p = new StoreProperty("test_string", "test_val");
            string s = Utils.HPCString(p);
            Assert.Equal("test_val", s);

            // The function also can return an empty string for nulls
            // in theory, but that should never happen, and is hard
            // to fake the test.
        }

        /// <summary>
        /// HPCString returns a valid int from StoreProperty.
        /// </summary>
        [Fact]
        public void HPCStringNull_Works()
        {
            StoreProperty p = new StoreProperty("test_int", 256);
            int i = Utils.HPCInt(p);
            Assert.Equal(256, i);
        }
    }
}