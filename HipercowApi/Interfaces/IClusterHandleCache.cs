// Copyright (c) Imperial College London. All rights reserved.

namespace HipercowApi.Tools
{
    using Microsoft.Hpc.Scheduler;

    /// <summary>
    /// Interface wrapper for the ClusterHandleCache, which stores the
    /// mapping of name to an connected IScheduler handle.
    /// </summary>
    public interface IClusterHandleCache
    {
        /// <summary>
        /// Fetch information about a named cluster.
        /// </summary>
        /// <param name="cluster">The name of the cluster - currently only wpia-hn.</param>
        /// <returns>A IScheduler object if the named cluster exists, otherwise null.</returns>
        public IScheduler? GetClusterHandle(string cluster);
    }
}
