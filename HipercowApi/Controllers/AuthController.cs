// Copyright (c) Imperial College London. All rights reserved.

using System.DirectoryServices.ActiveDirectory;
using System.DirectoryServices.Protocols;
using System.Net;
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
        NetworkCredential credentials = _ldapManager.GetLdapCredentials(request);
        LdapConnection ldap = _ldapManager.GetLdapConnection(credentials);
        _ldapManager.DoBind(ldap, credentials);
        List<string> groups = _ldapManager.GetDomainGroups(request.Username, ldap);
        return Ok(_jwtSupport.GenerateEncryptedToken(
            request.Username,
            request.Password,
            groups.Contains("WPIA-HN.HPC Users - All Nodes"),
            groups.Contains("WPIA-HN.HPC Administrators")));
    }
}