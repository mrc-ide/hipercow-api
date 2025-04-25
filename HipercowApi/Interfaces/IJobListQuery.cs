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
        /// Look up jobs from the scheduler by user and state.
        /// </summary>
        /// <param name="cluster">The cluster name, currently wpia-hn.</param>
        /// <param name="scheduler">The scheduler object to query.</param>
        /// <param name="user">If set, filter results to this user.</param>
        /// <param name="state">If set, filter jobs to this state.</param>
        /// <param name="maxRows">Set maximum number of rows to return.</param>
        /// <returns>
        /// A JobList object containing the results.
        /// .</returns>
        public JobList GetJobList(
            string cluster,
            IScheduler scheduler,
            string? user,
            string? state,
            int? maxRows);
    }
}
