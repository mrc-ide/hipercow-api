// Copyright (c) Imperial College London. All rights reserved.

using System.DirectoryServices.Protocols;
using System.Net;
using HipercowApi.Tools;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// The auth/ endpoints for authentication.
/// </summary>
public class AuthController(JwtTokenGenerator tokenGenerator, UserSessionManager sessionManager) : ControllerBase
{
    private readonly JwtTokenGenerator tokenGenerator = tokenGenerator;
    private readonly string ldapServer = "wpia-didedc2.dide.ic.ac.uk";
    private readonly int ldapPort = 389;
    private readonly string domain = "dide.local";
    private readonly UserSessionManager sessionManager = sessionManager;

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

        try
        {
            LdapDirectoryIdentifier ldapDirId = new(this.ldapServer, this.ldapPort);
            LdapConnection ldap = new(ldapDirId);
            NetworkCredential credentials = new(request.Username, request.Password, this.domain);
            ldap.AuthType = AuthType.Negotiate;
            ldap.Bind(credentials);
            List<string> groups = Utils.GetDomainGroups(request.Username, ldap);
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
        catch (LdapException)
        {
            return this.Unauthorized("Invalid credentials.");
        }
        catch (Exception ex)
        {
            return this.StatusCode(500, $"Internal error: {ex.Message}");
        }
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
        return this.Ok();
    }
}