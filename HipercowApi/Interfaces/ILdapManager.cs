// Copyright (c) Imperial College London. All rights reserved.

namespace HipercowApi.Tools
{
    using System.DirectoryServices.Protocols;
    using System.Net;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Interface wrapper for LdapManager which authenticates against
    /// the DIDE domain.
    /// </summary>
    public interface ILdapManager
    {
        /// <summary>
        /// Create a network credentials object from the login request.
        /// </summary>
        /// <param name="request">The login details (username and password).
        /// They cannot both be empty.</param>
        /// <returns>A network credential object.</returns>
        public NetworkCredential GetLdapCredentials(LoginRequest request);

        /// <summary>
        /// Create an Ldap connection ready to attempt binding with.
        /// </summary>
        /// <param name="creds">
        /// The network credentials created by GetLdapCredentials.
        /// </param>
        /// <returns>
        /// A LdapConnection object.
        /// </returns>
        public LdapConnection GetLdapConnection(NetworkCredential creds);

        /// <summary>
        /// Using LDAP, query and parse a list of groups that a DIDE
        /// domain user belongs to.
        /// </summary>
        /// <param name="user">The username.</param>
        /// <param name="ldap">A LdapConnection.</param>
        /// <returns>
        /// A list of names of groups.
        /// </returns>
        public List<string> GetDomainGroups(string user, LdapConnection ldap);

        /// <summary>
        /// Try to bind to an LDAP server using credentials.
        /// </summary>
        /// <param name="ldap">The LdapConnection object.</param>
        /// <param name="credentials">The NetworkCredential object.</param>
        public void DoBind(LdapConnection ldap, NetworkCredential credentials);
    }
}
