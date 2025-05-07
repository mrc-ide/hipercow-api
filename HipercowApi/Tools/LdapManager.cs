// Copyright (c) Imperial College London. All rights reserved.

using System.Diagnostics.CodeAnalysis;
using System.DirectoryServices.Protocols;
using System.Net;
using HipercowApi.Tools;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Wrapper for creationg an LdapConnection, so we can mock/test.
/// </summary>
[ExcludeFromCodeCoverage]
public class LdapManager : ILdapManager
{
    private static readonly string LdapServer = "wpia-didedc2.dide.ic.ac.uk";
    private static readonly int LdapPort = 389;
    private static readonly string Domain = "dide.local";

    // Excluded from code coverage as this part is DIDE specific, which
    // we don't want to attempt from CI. It doesn't seem worth mocking.

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    public LdapConnectionWrapper GetDideLdapConnection(ControllerBase controller, LoginRequest request)
    {
        try
        {
            LdapDirectoryIdentifier ldapDirId = new(LdapServer, LdapPort);
            LdapConnection ldap = new(ldapDirId);
            NetworkCredential credentials = new(request.Username, request.Password, Domain);
            ldap.AuthType = AuthType.Negotiate;
            ldap.Bind(credentials);
            return new LdapConnectionWrapper
            {
                Connection = ldap,
                Result = controller.Ok(),
            };
        }
        catch (LdapException)
        {
            return new LdapConnectionWrapper
            {
                Connection = null,
                Result = controller.Unauthorized("Invalid credentials."),
            };
        }
        catch (Exception ex)
        {
            return new LdapConnectionWrapper
            {
                Connection = null,
                Result = controller.StatusCode(500, $"Internal error: {ex.Message}"),
            };
        }
    }

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    public List<string> GetDomainGroups(string user, LdapConnection ldap)
    {
        List<string> groups = [];
        SearchRequest searchRequest = new(
            "OU=Users,OU=DIDE Users,DC=dide,DC=local",
            "(&(objectCategory=person)(SAMAccountName=" + user + "))",
            SearchScope.Subtree,
            ["SAMAccountName", "memberOf", "cn"]);

        SearchResponse searchResponse = (SearchResponse)ldap.SendRequest(searchRequest);

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
                        groups.Add(result_split[j].Substring(3));
                    }
                }
            }
        }

        return groups;
    }
}
