// Copyright (c) Imperial College London. All rights reserved.

namespace HipercowApi.Tools
{
    using HipercowApi.Models;
    using Microsoft.Hpc.Scheduler;

    /// <summary>
    /// Interface wrapper for GetClusterLoad, which fetches a list of nodes and
    /// their current load, for a given cluster. (Currently only wpia-hn.)
    /// </summary>
    public interface IClusterLoadQuery
    {
        /// <summary>
        /// Fetch information about the current load of a named cluster.
        /// </summary>
        /// <param name="cluster">
        /// The name of the cluster - currently only wpia-hn.
        /// </param>
        /// <param name="scheduler">
        /// The connected scheduler object to query.
        /// </param>
        /// <returns>
        /// A ClusterLoad object. (The cluster is sure to exist
        /// if we get this far)
        /// .</returns>
        public ClusterLoad GetClusterLoad(string cluster, IScheduler scheduler);
    }
}
