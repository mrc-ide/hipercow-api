// Copyright (c) Imperial College London. All rights reserved.

namespace HipercowApi.Controllers
{
    using HipercowApi.Tools;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Hpc.Scheduler;

    /// <summary>
    /// The /clusters and /clusters/xxx endpoints provide the list of clusters
    /// and information about a particular cluster respectively.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="ClusterLoadController"/> class.
    /// </remarks>
    /// <param name="clusterLoadQuery">
    /// The ClusterLoadQuery object for dependency injection.
    /// Contains GetClusterLoad function.
    /// </param>
    /// <param name="clusterHandleCache">The ClusterHandleCache object so we can
    /// retrieve the connected scheduler object for the requested cluster.
    /// </param>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ClusterLoadController(
        IClusterLoadQuery clusterLoadQuery,
        IClusterHandleCache clusterHandleCache) : ControllerBase
    {
        private readonly IClusterLoadQuery _clusterLoadQuery = clusterLoadQuery;
        private readonly IClusterHandleCache _clusterHandleCache = clusterHandleCache;

        /// <summary>
        /// Endpoint to return current load of a particular cluster.
        /// </summary>
        /// <param name="cluster">The name of the cluster to query.</param>
        /// <returns>
        /// The information about the cluster load (see clusterLoadQuery) wrapped
        /// in an IActionResult to indicate whether the request was ok or not. The
        /// NotFound status is returned if the cluster requested does not exist.
        /// </returns>
        [HttpGet("{cluster}")]
        public IActionResult Get(string cluster)
        {
            IScheduler? scheduler = _clusterHandleCache.GetClusterHandle(cluster);
            return scheduler is null ?
                NotFound() :
                Ok(_clusterLoadQuery.GetClusterLoad(cluster, scheduler));
        }
    }
}
