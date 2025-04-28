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
    /// <param name="Id">Gets or sets the job id.</param>
    /// <param name="State">Gets or sets the job state.</param>
    /// <param name="JobName">Gets or sets the name of the job.</param>
    [ExcludeFromCodeCoverage]
    public record JobInfo(
        int Id,
        string State,
        string JobName);
}
