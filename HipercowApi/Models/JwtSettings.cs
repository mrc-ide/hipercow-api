// Copyright (c) Imperial College London. All rights reserved.

/// <summary>
/// The JWT Settings wrapper.
/// </summary>
public class JwtSettings
{
    /// <summary>
    /// Gets or sets the key.
    /// </summary>
    public required string SecretKey { get; set; }

    /// <summary>
    /// Gets or sets the issuer.
    /// </summary>
    public required string Issuer { get; set; }

    /// <summary>
    /// Gets or sets the audience, whatever that is.
    /// </summary>
    public required string Audience { get; set; }

    /// <summary>
    /// Gets or sets the token expiry time.
    /// </summary>
    public required int ExpiresInMinutes { get; set; }
}
