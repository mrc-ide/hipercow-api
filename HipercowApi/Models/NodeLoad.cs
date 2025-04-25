// Copyright (c) Imperial College London. All rights reserved.

namespace HipercowApi.Models
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// A class containing information about a node's current load.
    /// </summary>
    /// <param name="Name">The name of the node.</param>
    /// <param name="CoresInUse">The number of cores in use.</param>
    /// <param name="NodeCores">The total cores on the node.</param>
    /// <param name="State">Current node state as a string.</param>
    [ExcludeFromCodeCoverage]
    public record NodeLoad(
        string Name,
        int CoresInUse,
        int NodeCores,
        string State);
}
