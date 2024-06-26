// Copyright (c) Imperial College London. All rights reserved.

namespace Hipercow_api.Tools
{
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.Hpc.Scheduler.Properties;

    /// <summary>
    /// Miscellaneous support helper functions to make the code more readable elsewhere.
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Wrapper around Microsoft.Hpc.Scheduler.Properties; return a string if
        /// possible, otherwise return an empty string, rather than null. In reality,
        /// a non-null StoreProperty can't have a null value or ToString - but I
        /// think it is not declared that way - hence code coverage disabled for
        /// this one-liner, as otherwise I get a partial line coverage that I can't
        /// complete.
        /// </summary>
        /// <param name="sp">A Microsoft.Hpc.Scheduler.Properties.StoreProperty, returned when querying the cluster.</param>
        /// <returns>A string which either has a value or is empty, but is not null.</returns>
        [ExcludeFromCodeCoverage]
        public static string HPCString(StoreProperty sp)
        {
            return sp.Value?.ToString() ?? string.Empty;
        }

        /// <summary>
        /// Wrapper around Microsoft.Hpc.Scheduler.Properties; return an int if
        /// possible. This is guaranteed to be an int for all valid usage.
        /// </summary>
        /// <param name="sp">A Microsoft.Hpc.Scheduler.Properties.StoreProperty, returned when querying the cluster.</param>
        /// <returns>An integer value for the property.</returns>
        public static int HPCInt(StoreProperty sp)
        {
            return int.Parse(HPCString(sp));
        }
    }
}
