// Copyright (c) Imperial College London. All rights reserved.

namespace HipercowApi.Controllers
{
    using HipercowApi.Tools;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Hpc.Scheduler;

    /// <summary>
    /// The /joblist endpoint.
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class JobListController : ControllerBase
    {
        private readonly IClusterHandleCache clusterHandleCache;
        private readonly IJobListQuery jobListQuery;

        /// <summary>
        /// Initializes a new instance of the <see cref="JobListController"/> class.
        /// </summary>
        /// <param name="jobListQuery">
        /// The JobListQuery object for dependency injection.
        /// Contains GetJobList function.
        /// </param>
        /// <param name="clusterHandleCache">The ClusterHandleCache object so we can
        /// retrieve the connected scheduler object for the requested cluster.
        /// </param>
        public JobListController(
            IJobListQuery jobListQuery,
            IClusterHandleCache clusterHandleCache)
        {
            this.jobListQuery = jobListQuery;
            this.clusterHandleCache = clusterHandleCache;
        }

        /// <summary>
        /// Endpoint to return a list of jobs and information about them.
        /// </summary>
        /// <param name="cluster">The cluster to query.</param>
        /// <param name="user">Optional: Filter jobs to this user.</param>
        /// <param name="state">Optional: Filter jobs to this state.</param>
        /// <param name="maxRows">Optional: The maximum number of rows to return.</param>
        /// <returns>
        /// The information about the cluster load (see clusterLoadQuery) wrapped
        /// in an IActionResult to indicate whether the request was ok or not. The
        /// NotFound status is returned if the cluster requested does not exist.
        /// </returns>
        [HttpPost]
        public IActionResult Post(
            [FromForm] string cluster,
            [FromForm] string? user,
            [FromForm] string? state,
            [FromForm] int? maxRows)
        {
            if ((state is not null) && (Utils.HPCJobState(state) is null))
            {
                return this.BadRequest("Job State " + state + " not found. " +
                    "Options: Canceled, Failed, Finished, Queued, Running or leave empty.");
            }

            IScheduler? scheduler = this.clusterHandleCache.GetClusterHandle(cluster);
            return scheduler is null ?
                this.NotFound() :
                this.Ok(this.jobListQuery.GetJobList(
                    cluster, scheduler, user, state, maxRows));
        }
    }
}
