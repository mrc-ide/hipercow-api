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
    private static readonly string _LdapServer = "wpia-didedc2.dide.ic.ac.uk";
    private static readonly int _LdapPort = 389;
    private static readonly string _Domain = "dide.local";

    // Excluded from code coverage as this part is DIDE specific, which
    // we don't want to attempt from CI. It doesn't seem worth mocking.

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    public LdapConnection? GetDideLdapConnection(LoginRequest request)
    {
        try
        {
            LdapDirectoryIdentifier ldapDirId = new(_LdapServer, _LdapPort);
            LdapConnection ldap = new(ldapDirId);
            NetworkCredential credentials = new(request.Username, request.Password, _Domain);
            ldap.AuthType = AuthType.Negotiate;
            ldap.Bind(credentials);
            return ldap;
        }
        catch (LdapException)
        {
            return null;
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
                foreach (string group in result_part.Split([',']).Where(g => g.StartsWith("CN=")))
                {
                    groups.Add(group.Substring(3));
                }
            }
        }

        return groups;
    }
}
