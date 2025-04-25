// Copyright (c) Imperial College London. All rights reserved.

namespace HipercowApi.Models
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// A class containing information about a cluster.
    /// </summary>
    /// <param name="Name">The name of the cluster.</param>
    /// <param name="MaxRam">The largest RAM (Gb) for any node.</param>
    /// <param name="MaxCores">The maximum cores for any node.</param>
    /// <param name="Nodes">A list of each node's name.</param>
    /// <param name="Queues">
    /// A list of queues that users (if they have permission) can
    /// submit jobs to.
    /// </param>
    /// <param name="DefaultQueue">
    /// The default queue that jobs are submitted to if the user does
    /// not specify one.
    /// </param>
    [ExcludeFromCodeCoverage]
    public record ClusterInfo(
            string Name,
            int MaxRam,
            int MaxCores,
            List<string> Nodes,
            List<string> Queues,
            string DefaultQueue);
}
