// Copyright (c) Imperial College London. All rights reserved.

namespace HipercowApi.Models
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// A class containing information about a cluster's current load.
    /// </summary>
    /// <param name="Name">The name of the requested cluster.</param>
    /// <param name="NodeLoads">
    /// A list of NodeLoad objects, giving the current number of busy cores,
    /// total number of cores, and state (online or offline) of each node.
    /// </param>
    [ExcludeFromCodeCoverage]
    public record ClusterLoad(
        string Name,
        List<NodeLoad> NodeLoads);
}
