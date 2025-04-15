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
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ClustersController : ControllerBase
    {
        private readonly IClusterInfoQuery clusterInfoQuery;
        private readonly IClusterHandleCache clusterHandleCache;

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="ClustersController"/> class.
        /// </summary>
        /// <param name="clusterInfoQuery">
        /// The cluster info query object for dependency injection.
        /// Contains GetClusterInfo function.
        /// </param>
        /// <param name="clusterHandleCache">The cluster handle cache so we can look up
        /// the connected scheduler object for the requested cluster.
        /// </param>
        public ClustersController(
            IClusterInfoQuery clusterInfoQuery,
            IClusterHandleCache clusterHandleCache)
        {
            this.clusterInfoQuery = clusterInfoQuery;
            this.clusterHandleCache = clusterHandleCache;
        }

        /// <summary>
        /// Endpoint to return the list of available clusters.
        /// </summary>
        /// <returns>
        /// A list of cluster names - currently hard coded into the API.
        /// </returns>
        [HttpGet]
        public List<string> Get()
        {
            return DideConstants.GetDideClusters();
        }

        /// <summary>
        /// Endpoint to return information about a particular cluster.
        /// </summary>
        /// <param name="cluster">The name of the cluster to query.</param>
        /// <returns>
        /// The information about the cluster (see clusterInfoQuery) wrapped
        /// in an IActionResult to indicate whether the request was ok or not.
        /// The NotFound status is returned if the cluster requested does not
        /// exist.
        /// </returns>
        [HttpGet("{cluster}")]
        public IActionResult Get(string cluster)
        {
            IScheduler? scheduler = this.clusterHandleCache.GetClusterHandle(cluster)!;
            return scheduler is null ?
                this.NotFound() :
                this.Ok(this.clusterInfoQuery.GetClusterInfo(cluster, scheduler));
        }
    }
}
