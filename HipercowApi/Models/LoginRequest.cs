// Copyright (c) Imperial College London. All rights reserved.

/// <summary>
/// A simple wrapper for a login request.
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// Gets or sets the username for logging in.
    /// </summary>
    public required string Username { get; set; }

    /// <summary>
    /// Gets or sets the password for logging in.
    /// </summary>
    public required string Password { get; set; }
}
