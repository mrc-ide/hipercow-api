// Copyright (c) Imperial College London. All rights reserved.

namespace HipercowApi.Models
{
    /// <summary>
    /// A class containing information about a cluster's current load.
    /// </summary>
    public class ClusterLoad
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClusterLoad"/> class.
        /// </summary>
        /// <param name="name">The name of the requested cluster.</param>
        /// <param name="nodeLoads">
        /// A list of NodeLoad objects, giving the current number of busy cores,
        /// total number of cores, and state (online or offline) of each node.
        /// </param>
        public ClusterLoad(
            string name,
            List<NodeLoad> nodeLoads)
        {
            this.Name = name;
            this.NodeLoads = nodeLoads;
        }

        /// <summary>
        /// Gets or sets the name of the cluster.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the list of NodeLoad objects.
        /// </summary>
        public List<NodeLoad> NodeLoads { get; set; }
    }
}
