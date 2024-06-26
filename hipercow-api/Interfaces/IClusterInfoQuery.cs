// Copyright (c) Imperial College London. All rights reserved.

namespace Hipercow_api.Tools
{
    using Hipercow_api.Models;

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
        /// <param name="scheduler">Mainly for testing - the scheduler object to query.</param>
        /// <returns>A ClusterInfo object if the named cluster exists, otherwise null.</returns>
        public ClusterInfo? GetClusterInfo(string cluster, IHipercowScheduler? scheduler = null);
    }
}
