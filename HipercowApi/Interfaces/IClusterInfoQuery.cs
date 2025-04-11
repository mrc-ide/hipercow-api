// Copyright (c) Imperial College London. All rights reserved.

namespace HipercowApi.Tools
{
    using HipercowApi.Models;
    using Microsoft.Hpc.Scheduler;

    /// <summary>
    /// Interface wrapper for GetClusterInfo, which fetches cluster
    /// information about a given cluster. (Currently only wpia-hn.)
    /// </summary>
    public interface IClusterInfoQuery
    {
        /// <summary>
        /// Fetch information about a named cluster.
        /// </summary>
        /// <param name="cluster">The name of the cluster - currently only wpia-hn.</param>
        /// <param name="scheduler">The handle to the scheduler.</param>
        /// <param name="utils">Reference to utils for mocking...</param>
        /// <returns>A ClusterInfo object if the named cluster exists, otherwise null.</returns>
        public ClusterInfo GetClusterInfo(string cluster, IScheduler scheduler, IUtils utils);
    }
}
