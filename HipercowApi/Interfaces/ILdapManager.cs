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
        /// <param name="controller">
        /// The API controller making the call.
        /// </param>
        /// <param name="request">
        /// The login request (user, password).
        /// </param>
        /// <returns>
        /// A LdapConnectionWrapper object, which contains a
        /// connection (if possible to create), and a web result code.
        /// </returns>
        public LdapConnectionWrapper GetDideLdapConnection(ControllerBase controller, LoginRequest request);

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
