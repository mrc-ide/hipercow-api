// Copyright (c) Imperial College London. All rights reserved.

namespace HipercowApi.Models
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// A class containing a cluster name, and list of jobs queried from it.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="JobList"/> class.
    /// </remarks>
    /// <param name="name">The name of the requested cluster.</param>
    /// <param name="jobs">
    /// A list of JobInfo objects, giving information on each job.
    /// </param>
    public class JobList(
        List<JobInfo> jobs)
    {
        /// <summary>
        /// Gets or sets the list of NodeLoad objects.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public List<JobInfo> Jobs { get; set; } = jobs;
    }
}
