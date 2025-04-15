// Copyright (c) Imperial College London. All rights reserved.

namespace HipercowApi.Tools
{
    using HipercowApi.Models;
    using Microsoft.Hpc.Scheduler;
    using Microsoft.Hpc.Scheduler.Properties;

    /// <summary>
    /// Interface wrapper for GetClusterInfo, which fetches cluster
    /// information about a given cluster. (Currently only wpia-hn.)
    /// </summary>
    public interface IClusterInfoQuery
    {
        /// <summary>
        /// Fetch information about a named cluster.
        /// </summary>
        /// <param name="cluster">The name of the cluster (currently only wpia-hn).</param>
        /// <param name="scheduler">The handle to the connected scheduler.</param>
        /// <returns>A ClusterInfo object. (The cluster is sure to exist if it gets this far).</returns>
        public ClusterInfo GetClusterInfo(string cluster, IScheduler scheduler);
    }
}
