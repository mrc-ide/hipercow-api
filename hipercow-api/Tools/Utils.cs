// Copyright (c) Imperial College London. All rights reserved.

namespace Hipercow_api.Tools
{
    using Microsoft.Hpc.Scheduler.Properties;

    /// <summary>
    /// Miscellaneous support helper functions to make the code more readable elsewhere.
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Wrapper around Microsoft.Hpc.Scheduler.Properties; return a string if
        /// possible, otherwise return an empty string, rather than null.
        /// </summary>
        /// <param name="sp">A Microsoft.Hpc.Scheduler.Properties.StoreProperty, returned when querying the cluster.</param>
        /// <returns>A string which either has a value or is empty, but is not null.</returns>
        public static string HPCString(StoreProperty sp)
        {
            string res = string.Empty;
            if (sp.Value != null)
            {
                if (sp.Value.ToString() != null)
                {
                    res = sp.Value.ToString()!;
                }
            }

            return res;
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
