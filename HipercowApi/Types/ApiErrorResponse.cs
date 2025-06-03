// Copyright (c) Imperial College London. All rights reserved.

namespace HipercowApi.Types
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Class for Api Error Response.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ApiErrorResponse
    {
        /// <summary>
        /// Gets or Sets the status code.
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Gets or Sets the message string.
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Gets or Sets the details.
        /// </summary>
        public string Details { get; set; } = string.Empty;

        /// <summary>
        /// Gets or Sets the timestamp.
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}