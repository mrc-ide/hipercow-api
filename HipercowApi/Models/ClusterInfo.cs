// Copyright (c) Imperial College London. All rights reserved.

namespace HipercowApi.Models
{
    /// <summary>
    /// A class containing information about a cluster.
    /// </summary>
    public class ClusterInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClusterInfo"/> class.
        /// The constructor for ClusterInfo.
        /// </summary>
        /// <param name="name">The name of the cluster.</param>
        /// <param name="maxRam">The largest RAM (Gb) for any node.</param>
        /// <param name="maxCores">The maximum cores for any node.</param>
        /// <param name="nodes">A list of each node's name.</param>
        /// <param name="queues">
        /// A list of queues that users (if they have permission) can
        /// submit jobs to.
        /// </param>
        /// <param name="defaultQueue">
        /// The default queue that jobs are submitted to if the user does
        /// not specify one.
        /// </param>
        public ClusterInfo(
            string name,
            int maxRam,
            int maxCores,
            List<string> nodes,
            List<string> queues,
            string defaultQueue)
        {
            this.Name = name;
            this.MaxRam = maxRam;
            this.MaxCores = maxCores;
            this.Nodes = nodes;
            this.Queues = queues;
            this.DefaultQueue = defaultQueue;
        }

        /// <summary>
        /// Gets or sets the name of the cluster.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the maximum available RAM in any node.
        /// </summary>
        public int MaxRam { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of cores in any node.
        /// </summary>
        public int MaxCores { get; set; }

        /// <summary>
        /// Gets or sets the list of node names.
        /// </summary>
        public List<string> Nodes { get; set; }

        /// <summary>
        /// Gets or sets the list of queues that users with
        /// appropriate permissions can use.
        /// </summary>
        public List<string> Queues { get; set; }

        /// <summary>
        /// Gets or sets the default queue used for submitting jobs
        /// if no specific queue is specified.
        /// </summary>
        public string DefaultQueue { get; set; }
    }
}
