// Copyright (c) Imperial College London. All rights reserved.

namespace Hipercow_api.Controllers
{
    using Hipercow_api.Models;
    using Hipercow_api.Tools;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// The /clusters and /clusters/xxx endpoints provide the list of clusters
    /// and information about a particular cluster respectively.
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ClustersController : ControllerBase
    {
        /// <summary>
        /// An object implementing IClusterInfoQuery, responsible for returning
        /// the list of clusters, or information on a specific cluster.
        /// </summary>
        private IClusterInfoQuery clusterInfoQuery = new ClusterInfoQuery();

        /// <summary>
        /// Used for testing. Allow injecting an IClusterInfoQuery object
        /// returning defined results, rather than doing a genuine query.
        /// </summary>
        /// <param name="inject">A mock cluster info query.</param>
        [NonAction]
        public void MockClusterInfoQuery(IClusterInfoQuery inject)
        {
            this.clusterInfoQuery = inject;
        }

        /// <summary>
        /// Endpoint to return the list of available clusters.
        /// </summary>
        /// <returns>
        /// A list of cluster names.
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
            ClusterInfo? info = this.clusterInfoQuery.GetClusterInfo(cluster);
            if (info != null)
            {
                return this.Ok(info);
            }

            return this.NotFound();
        }
    }
}
