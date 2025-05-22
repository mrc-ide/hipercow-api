// Copyright (c) Imperial College London. All rights reserved.

using System.Diagnostics.CodeAnalysis;
using System.DirectoryServices.Protocols;
using System.Net;
using Hipercow_api.Tools.Exceptions;
using HipercowApi.Tools;

/// <summary>
/// Wrapper for creationg an LdapConnection, so we can mock/test.
/// </summary>
[ExcludeFromCodeCoverage]
public class LdapManager : ILdapManager
{
    private static readonly string _LdapServer = "wpia-didedc2.dide.ic.ac.uk";
    private static readonly int _LdapPort = 389;
    private static readonly string _Domain = "dide.local";

    /// <inheritdoc/>
    public NetworkCredential GetLdapCredentials(LoginRequest request)
    {
        if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
        {
            throw new LdapEmptyUsernamePassword();
        }

        return new NetworkCredential(request.Username, request.Password, _Domain);
    }

    /// <inheritdoc/>
    public LdapConnection GetLdapConnection(NetworkCredential creds)
    {
        LdapDirectoryIdentifier ldapDirId = new(_LdapServer, _LdapPort);
        LdapConnection ldap = new(ldapDirId)
        {
            AuthType = AuthType.Negotiate,
        };
        return ldap;
    }

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    public void DoBind(LdapConnection ldap, NetworkCredential credentials)
    {
        try
        {
            ldap.Bind(credentials);
        }
        catch
        {
            throw new LdapAuthFailure(credentials.UserName);
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
        if (searchResponse.Entries.Count == 0)
        {
            return groups;
        }

        SearchResultEntry item = searchResponse.Entries[0];
        if (!item.Attributes.Contains("memberOf"))
        {
            return groups;
        }

        for (int i = 0; i < item.Attributes["memberOf"].Count; i++)
        {
            string result_part = item.Attributes["memberOf"][i].ToString()!;
            foreach (string group in result_part.Split([',']).Where(g => g.StartsWith("CN=")))
            {
                groups.Add(group.Substring(3));
            }
        }

        return groups;
    }
}
