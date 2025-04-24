// Copyright (c) Imperial College London. All rights reserved.

namespace HipercowApi.Models
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Information about an existing cluster job.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="JobInfo"/> class.
    /// </remarks>
    /// <param name="id">Gets or sets the job id.</param>
    /// <param name="state">Gets or sets the job state.</param>
    /// <param name="jobname">Gets or sets the name of the job.</param>
    public class JobInfo(
        int id, string state, string jobname)
    {
        /// <summary>
        /// Gets or sets the Job Id.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public int Id { get; set; } = id;

        /// <summary>
        /// Gets or sets the Job State.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public string State { get; set; } = state;

        /// <summary>
        /// Gets or sets the Job State.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public string JobName { get; set; } = jobname;
    }
}
