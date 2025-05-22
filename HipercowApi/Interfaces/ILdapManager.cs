// Copyright (c) Imperial College London. All rights reserved.

namespace HipercowApi.Tools
{
    using System.DirectoryServices.Protocols;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Interface wrapper for LdapManager which authenticates against
    /// the DIDE domain.
    /// </summary>
    public interface ILdapManager
    {
        /// <summary>
        /// Authenticate against the DIDE domain.
        /// </summary>
        /// <param name="request">
        /// The login request (user, password).
        /// </param>
        /// <returns>
        /// A LdapConnection object, or a null if it failed to
        /// connect or authenticate.
        /// </returns>
        public LdapConnection? GetDideLdapConnection(LoginRequest request);

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
    }
}
