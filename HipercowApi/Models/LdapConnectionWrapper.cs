// Copyright (c) Imperial College London. All rights reserved.

using System.DirectoryServices.Protocols;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// A simple wrapper over an LdapConnection,
/// or the error returned trying to make one.
/// </summary>
public class LdapConnectionWrapper
{
    /// <summary>
    /// Gets or sets the username for logging in.
    /// </summary>
    public required LdapConnection? Connection { get; set; }

    /// <summary>
    /// Gets or sets the password for logging in.
    /// </summary>
    public required IActionResult Result { get; set; }
}
