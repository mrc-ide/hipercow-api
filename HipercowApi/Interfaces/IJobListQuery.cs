// Copyright (c) Imperial College London. All rights reserved.

namespace HipercowApi.Tools
{
    using HipercowApi.Models;
    using Microsoft.Hpc.Scheduler;

    /// <summary>
    /// Interface wrapper for GetJobList, which fetches a list of jobs for
    /// a user, for a given cluster. (Currently only wpia-hn.)
    /// </summary>
    public interface IJobListQuery
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
        /// <param name="user">
        /// If set, filter results to this user.
        /// </param>
        /// <param name="state">
        /// If set, filter jobs to this state.
        /// </param>
        /// <param name="maxRows">
        /// Set maximum number of rows to return.
        /// </param>
        /// <returns>
        /// A JobList object. (The cluster is sure to exist
        /// if we get this far)
        /// .</returns>
        public JobList GetJobList(string cluster, IScheduler scheduler, string user, string state, int maxRows);
    }
}
