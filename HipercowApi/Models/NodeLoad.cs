// Copyright (c) Imperial College London. All rights reserved.

namespace HipercowApi.Models
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// A class containing information about a node's current load.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="NodeLoad"/> class.
    /// </remarks>
    /// <param name="name">The name of the node.</param>
    /// <param name="coresInUse">The number of cores in use.</param>
    /// <param name="nodeCores">The total cores on the node.</param>
    /// <param name="state">Current node state as a string.</param>
    public class NodeLoad(
        string name,
        int coresInUse,
        int nodeCores,
        string state)
    {
        /// <summary>
        /// Gets or sets the name of the node.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public string Name { get; set; } = name;

        /// <summary>
        /// Gets or sets the number of cores in use.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public int CoresInUse { get; set; } = coresInUse;

        /// <summary>
        /// Gets or sets the number of cores the node has.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public int NodeCores { get; set; } = nodeCores;

        /// <summary>
        /// Gets or sets the current state of the node.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public string State { get; set; } = state;
    }
}
