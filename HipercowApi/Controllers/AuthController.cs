// Copyright (c) Imperial College London. All rights reserved.

using System.DirectoryServices.Protocols;
using HipercowApi.Tools;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// The auth/ endpoints for authentication.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class AuthController(JwtSupport jwtSupport, ILdapManager ldapManager) : ControllerBase
{
    private readonly JwtSupport _jwtSupport = jwtSupport;
    private readonly ILdapManager _ldapManager = ldapManager;

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
            return BadRequest("Username or password cannot be empty.");
        }

        LdapConnection? ldap = _ldapManager.GetDideLdapConnection(request);
        if (ldap is null)
        {
            return Unauthorized("Failed to login");
        }

        List<string> groups = _ldapManager.GetDomainGroups(request.Username, ldap);
        bool wpia_user = groups.Contains("WPIA-HN.HPC Users - All Nodes");
        bool wpia_admin = groups.Contains("WPIA-HN.HPC Administrators");
        if (!wpia_user && !wpia_admin)
        {
            return BadRequest();
        }

        return Ok(_jwtSupport.GenerateEncryptedToken(
            request.Username,
            request.Password,
            wpia_user,
            wpia_admin));
    }
}