// Copyright (c) Imperial College London. All rights reserved.

namespace HipercowApi.Models
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// A class containing information about a cluster's current load.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="JobList"/> class.
    /// </remarks>
    /// <param name="name">The name of the requested cluster.</param>
    /// <param name="jobs">
    /// A list of JobInfo objects, giving information on each job.
    /// </param>
    public class JobList(
        string name,
        List<JobInfo> jobs)
    {
        /// <summary>
        /// Gets or sets the name of the cluster.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public string Name { get; set; } = name;

        /// <summary>
        /// Gets or sets the list of NodeLoad objects.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public List<JobInfo> Jobs { get; set; } = jobs;
    }
}
