// Copyright (c) Imperial College London. All rights reserved.

namespace HipercowApi.Controllers
{
    using HipercowApi.Tools;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// The /clusters and /clusters/xxx endpoints provide the list of clusters
    /// and information about a particular cluster respectively.
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ClusterLoadController : ControllerBase
    {
        /// <summary>
        /// An object implementing IClusterLoadQuery, responsible for returning
        /// the current load of nodes on a specific cluster.
        /// </summary>
        private IClusterLoadQuery clusterLoadQuery = new ClusterLoadQuery();

        /// <summary>
        /// Initializes a new instance of the <see cref="ClusterLoadController"/> class.
        /// </summary>
        /// <param name="clusterLoadQuery">For testing only.</param>
        public ClusterLoadController(IClusterLoadQuery clusterLoadQuery)
        {
            this.clusterLoadQuery = clusterLoadQuery;
        }

        /// <summary>
        /// Endpoint to return current load of a particular cluster.
        /// </summary>
        /// <param name="cluster">The name of the cluster to query.</param>
        /// <returns>
        /// The information about the cluster load (see clusterLoadQuery) wrapped
        /// in an IActionResult to indicate whether the request was ok or not.
        /// The NotFound status is returned if the cluster requested does not
        /// exist.
        /// </returns>
        [HttpGet("{cluster}")]
        public IActionResult Get(string cluster)
        {
            var info = this.clusterLoadQuery.GetClusterLoad(cluster);
            if (info != null)
            {
                return this.Ok(info);
            }

            return this.NotFound();
        }
    }
}
