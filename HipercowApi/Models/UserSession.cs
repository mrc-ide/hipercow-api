// Copyright (c) Imperial College London. All rights reserved.

namespace HipercowApi.Models
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// A simple wrapper for session information.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public record UserSession
    {
        /// <summary>
        /// Gets or sets the session username.
        /// </summary>
        public required string Username { get; set; }

        /// <summary>
        /// Gets or sets the session password.
        /// </summary>
        public required string Password { get; set; } // Store encrypted, ideally

        /// <summary>
        /// Gets or sets a value indicating whether the user has access to wpia-hn.
        /// </summary>
        public required bool Wpia_hn_access { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user has admin rights to wpia-hn.
        /// </summary>
        public required bool Wpia_hn_admin { get; set; }
    }
}