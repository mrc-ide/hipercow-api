// Copyright (c) Imperial College London. All rights reserved.

using HipercowApi.Models;
using HipercowApi.Tools;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// The auth/ endpoints for authentication.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class AuthController(JwtTokenGenerator tokenGenerator, UserSessionManager sessionManager, ILdapManager ldapManager) : ControllerBase
{
    private readonly JwtTokenGenerator tokenGenerator = tokenGenerator;
    private readonly UserSessionManager sessionManager = sessionManager;
    private readonly ILdapManager ldapManager = ldapManager;

    /// <summary>
    /// The Login endpoint. Accept username and password and return a JWT and session ID.
    /// </summary>
    /// <param name="request">The request with Username and PAssword.</param>
    /// <returns>The token and session ID if successful, otherwise a standard error.</returns>
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
        bool wpia_hn_access = groups.Contains("WPIA-HN.HPC Users - All Nodes");
        bool wpia_hn_admin = groups.Contains("WPIA-HN.HPC Administrators");
        var token = this.tokenGenerator.GenerateToken(request.Username);
        var session = new UserSession
        {
            Username = request.Username,
            Password = request.Password,
            Wpia_hn_access = wpia_hn_access,
            Wpia_hn_admin = wpia_hn_admin,
        };
        var sessionId = this.sessionManager.StoreSession(session);
        return this.Ok(new { token, sessionId });
    }

    /// <summary>
    /// The Logout endpoint. Remove session.
    /// </summary>
    /// <param name="sessionId">The session id to close and logout.</param>
    /// <returns>The result, either Ok, or a standard error.</returns>
    [HttpPost("logout")]
    public IActionResult Logout([FromHeader(Name = "X-Session-Id")] string sessionId)
    {
        string? jwtUsername = this.User.Identity!.Name;
        UserSession? session = this.sessionManager.RetrieveSession(sessionId);
        IActionResult? result = Utils.CheckTokenAndSession(this, jwtUsername, session);
        if (result is not null)
        {
            return result;
        }

        this.sessionManager.RemoveSession(sessionId);
        return this.NoContent();
    }
}