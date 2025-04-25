// Copyright (c) Imperial College London. All rights reserved.

namespace HipercowApi.Models
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// A list of jobs queries from the cluster.
    /// </summary>
    /// <param name="Jobs">
    /// A list of JobInfo objects, giving information on each job.
    /// </param>
    [ExcludeFromCodeCoverage]
    public record JobList(
        List<JobInfo> Jobs);
}
