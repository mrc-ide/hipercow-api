// Copyright (c) Imperial College London. All rights reserved.

using HipercowApi.Models;
using HipercowApi.Tools;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// The auth/ endpoints for authentication.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class AuthController(JwtSupport jwtSupport, ILdapManager ldapManager) : ControllerBase
{
    private readonly JwtSupport jwtSupport = jwtSupport;
    private readonly ILdapManager ldapManager = ldapManager;

    /// <summary>
    /// The Login endpoint. Accept username and password and return a JWT and session ID.
    /// </summary>
    /// <param name="request">The request with Username and PAssword.</param>
    /// <returns>The token if successful, otherwise a standard error.</returns>
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
        {
            return this.BadRequest("Username or password cannot be empty.");
        }

        LdapConnectionWrapper ldap = this.ldapManager.GetDideLdapConnection(this, request);
        if (ldap.Connection == null)
        {
            return ldap.Result;
        }

        List<string> groups = this.ldapManager.GetDomainGroups(request.Username, ldap.Connection);
        var token = this.jwtSupport.GenerateEncryptedToken(
            request.Username,
            request.Password,
            groups.Contains("WPIA-HN.HPC Users - All Nodes"),
            groups.Contains("WPIA-HN.HPC Administrators"));

        return this.Ok(token);
    }
}