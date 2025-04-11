﻿// Copyright (c) Imperial College London. All rights reserved.

namespace HipercowApi.Tools
{
    using HipercowApi.Models;
    using Microsoft.Hpc.Scheduler;

    /// <summary>
    /// Interface wrapper for GetClusterLoad, which fetches the current
    /// load of nodes on a given cluster. (Currently only wpia-hn.)
    /// </summary>
    public interface IClusterLoadQuery
    {
        /// <summary>
        /// Fetch information about the current load of a named cluster.
        /// </summary>
        /// <param name="cluster">The name of the cluster - currently only wpia-hn.</param>
        /// <param name="scheduler">The scheduler to query.</param>
        /// <returns>A ClusterLoad object if the named cluster exists, otherwise null.</returns>
        public ClusterLoad? GetClusterLoad(string cluster, IScheduler scheduler);
    }
}
