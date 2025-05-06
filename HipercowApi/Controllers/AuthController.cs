// Copyright (c) Imperial College London. All rights reserved.

using System.DirectoryServices.Protocols;
using System.Net;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// The auth/ endpoints for authentication.
/// </summary>
public class AuthController(JwtTokenGenerator tokenGenerator, UserSessionManager sessionManager) : ControllerBase
{
    private static readonly string[] AttributeList = new string[] { "SAMAccountName", "memberOf", "cn" };

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
            using var ldapConnection = new LdapConnection(new LdapDirectoryIdentifier(this.ldapServer, this.ldapPort));
            var credentials = new NetworkCredential(request.Username, request.Password, this.domain);

            ldapConnection.AuthType = AuthType.Negotiate;
            ldapConnection.Bind(credentials); // Attempt bind

            SearchRequest searchRequest = new(
                    "OU=Users,OU=DIDE Users,DC=dide,DC=local",
                    "(&(objectCategory=person)(SAMAccountName=" + request.Username + "))",
                    SearchScope.Subtree,
                    AttributeList);

            SearchResponse searchResponse = (SearchResponse)ldapConnection.SendRequest(searchRequest);

            // Look for interesting groups
            bool wpia_hn_access = false;
            bool wpia_hn_admin = false;

            if (searchResponse.Entries.Count == 1)
            {
                SearchResultEntry item = searchResponse.Entries[0];
                for (int i = 0; i < item.Attributes["memberOf"].Count; i++)
                {
                    string result_part = item.Attributes["memberOf"][i].ToString()!;
                    string[] result_split = result_part.Split([',']);
                    for (int j = 0; j < (int)result_split.Length; j++)
                    {
                        if (result_split[j].StartsWith("CN"))
                        {
                            string group = result_split[j].Substring(3);
                            wpia_hn_access |= group == "WPIA-HN.HPC Users - All Nodes";
                            wpia_hn_admin |= group == "WPIA-HN.HPC Administrators";
                        }
                    }
                }
            }

            var token = this.tokenGenerator.GenerateToken(request.Username);
            var session = new UserSession { Username = request.Username, Password = request.Password, Wpia_hn_access = wpia_hn_access, Wpia_hn_admin = wpia_hn_admin };
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
}