// Copyright (c) Imperial College London. All rights reserved.

namespace HipercowApi.Models
{
    using Microsoft.Hpc.Scheduler.Properties;

    /// <summary>
    /// A class containing information about a node's current load.
    /// </summary>
    public class NodeLoad
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NodeLoad"/> class.
        /// </summary>
        /// <param name="name">The name of the node.</param>
        /// <param name="coresInUse">The number of cores in use.</param>
        /// <param name="nodeCores">The total cores on the node.</param>
        /// <param name="state">Current node state as a string.</param>
        public NodeLoad(
            string name,
            int coresInUse,
            int nodeCores,
            string state)
        {
            this.Name = name;
            this.CoresInUse = coresInUse;
            this.NodeCores = nodeCores;
            this.State = state;
        }

        /// <summary>
        /// Gets or sets the name of the node.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the number of cores in use.
        /// </summary>
        public int CoresInUse { get; set; }

        /// <summary>
        /// Gets or sets the number of cores the node has.
        /// </summary>
        public int NodeCores { get; set; }

        /// <summary>
        /// Gets or sets the current state of the node.
        /// </summary>
        public string State { get; set; }
    }
}
